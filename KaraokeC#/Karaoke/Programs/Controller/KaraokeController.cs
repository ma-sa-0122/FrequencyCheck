using Karaoke.Programs.Analysis;
using Karaoke.Programs.Audio;
using Karaoke.Programs.Data;
using Karaoke.Programs.Pitch;
using Karaoke.Programs.Song;
using Karaoke.Programs.TIme;
using Karaoke.Programs.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Karaoke.Programs.Controller
{
    internal class KaraokeController
    {
        private AudioInput input;
        private AudioPlayer player;

        private AnalysisConfig config;
        private AnalysisEngine analysis;
        private TimeManager timeManager;

        private SongManager songManager;

        private GuideRenderer guideRenderer;
        private Bitmap guideBitmap;
        private Dictionary<int, Bitmap> guideCache = new Dictionary<int, Bitmap>();

        private PitchBuffer pitchBuffer;

        private CancellationTokenSource cts;

        public ConcurrentQueue<double[]> drawQueue = new ConcurrentQueue<double[]>();
        public ConcurrentQueue<PitchResult> pitchQueue = new ConcurrentQueue<PitchResult>();

        private ConcurrentQueue<double[]> analyzeQueue = new ConcurrentQueue<double[]>();
        private ConcurrentQueue<double[]> audioQueue = new ConcurrentQueue<double[]>();

        private int samplingRate;
        private int samplingInterval;
        private int fftSize;

        public event Action OnPageViewNeedsUpdate;
        public event Action<string> OnLyricsChanged;

        public KaraokeController(int samplingRate, int samplingInterval, int fftSize)
        {
            this.samplingRate = samplingRate;
            this.samplingInterval = samplingInterval;
            this.fftSize = fftSize;

            input = new AudioInput(samplingInterval);
            player = new AudioPlayer();

            config = new AnalysisConfig();

            timeManager = new TimeManager(samplingRate, samplingInterval);
            songManager = new SongManager();

            guideRenderer = new GuideRenderer();

            pitchBuffer = new PitchBuffer(1024);

            input.OnFrame += OnFrame;

            songManager.OnPageChanged += _ =>
            {
                OnPageViewNeedsUpdate?.Invoke();
            };
            songManager.OnLyricsChanged += lyrics =>
            {
                OnLyricsChanged?.Invoke(lyrics);
            };
        }

        public void SetSamplingRate(int samplingRate)
        {
            this.samplingRate = samplingRate;
        }
        public void SetSamplingInterval(int samplingInterval)
        {
            this.samplingInterval = samplingInterval;
        }
        public void SetFftSize(int fftSize)
        {
            this.fftSize = fftSize;
        }
        public void SetLatency(double latency)
        {
            timeManager.SetLatency(latency);
        }


        private void OnFrame(double[] frame)
        {
            analyzeQueue.Enqueue(frame);
            audioQueue.Enqueue(frame);
        }

        public void Update()
        {
            while (pitchQueue.TryDequeue(out var p))
            {
                pitchBuffer.Add(p);
            }
        }


        public void Start(Func<double> musicTimeProvider, double latency, double minEnergy)
        {
            timeManager.SetMusicTimeProvider(musicTimeProvider);
            timeManager.SetLatency(latency);
            timeManager.Start();

            analysis = new AnalysisEngine(
                analyzeQueue,
                audioQueue,
                drawQueue,
                pitchQueue,
                timeManager,
                config,
                samplingRate,
                samplingInterval,
                latency
            );

            cts = new CancellationTokenSource();
            Task.Run(() => analysis.Run(cts.Token));

            input.Start();
        }

        public void Stop()
        {
            cts?.Cancel();
            input.Stop();
            player.Stop();
            timeManager.Stop();
        }


        /*
         楽曲関係
         */
        public void LoadMusic(string path, int latencyMs)
        {
            player.Load(path, latencyMs);
        }

        public void PlayMusic()
        {
            player.Play();
        }

        public void StopMusic()
        {
            player.Stop();
        }

        public double GetMusicTime()
        {
            if (player.IsPlaying)
                return player.CurrentTime;

            return 0;
        }

        public void Seek(double sec)
        {
            player.Seek(sec);
        }

        public void SetVolume(float volume)
        {
            player.SetVolume(volume);
        }


        /*
         ページ関係
         */
        public event Action<int> OnPageChanged
        {
            add { songManager.OnPageChanged += value; }
            remove { songManager.OnPageChanged -= value; }
        }

        public void LoadSong(SongData song)
        {
            ClearGuideCache();
            songManager.Load(song);
        }

        public void Update(double currentTime)
        {
            songManager.Update(currentTime);
        }

        public double GetPageStart() => songManager.PageStart;
        public double GetPageEnd() => songManager.PageEnd;

        public void MovePage(int delta)
        {
            var song = songManager.Song;
            if (song == null) return;

            int newPage = songManager.CurrentPage + delta;

            if (0 <= newPage && newPage < song.Pages.Count)
            {
                songManager.ForcePage(newPage);
            }
        }


        /*
         ガイド
         */
        public void RenderGuide(Chart chart)
        {
            var song = songManager.Song;
            int page = songManager.CurrentPage;

            if (song == null) return;

            // キャッシュヒット
            if (guideCache.TryGetValue(page, out var cached))
            {
                guideBitmap?.Dispose();
                guideBitmap = (Bitmap)cached.Clone();
                return;
            }

            // 新規生成
            var bmp = guideRenderer.RenderPage(chart, song, page);

            guideCache[page] = (Bitmap)bmp.Clone();

            guideBitmap?.Dispose();
            guideBitmap = bmp;
        }

        public Bitmap GetGuideBitmap()
        {
            return guideBitmap;
        }

        public void ClearGuideCache()
        {
            foreach (var bmp in guideCache.Values)
                bmp.Dispose();

            guideCache.Clear();
        }



        public double GetCurrentTime()
        {
            return timeManager.GetCurrentTime();
        }

        public void UpdateAnalysisConfig(bool useFourier, bool useHPS, double minEnergy)
        {
            config.UseFourier = useFourier;
            config.UseHPS = useHPS;
            config.MinEnergy = minEnergy;

            IPitchEstimator estimator = PitchEstimatorFactory.Create(useFourier, useHPS, samplingRate, fftSize);
            analysis?.SetEstimator(estimator);
        }




        /*
         ピッチ取得
         */
        public List<PitchResult> FetchNewPitches()
        {
            var list = new List<PitchResult>();

            while (pitchQueue.TryDequeue(out var p))
            {
                pitchBuffer.Add(p);  // 履歴にも保存
                list.Add(p);         // 描画用にも渡す
            }

            return list;
        }

        public void ClearPitchBuffer()
        {
            pitchBuffer.Clear();
        }


        /* 
         UI に描画用のデータを渡す
        */
        public bool TryGetWave(out double[] waveform)
        {
            return audioQueue.TryDequeue(out waveform);
        }

        public bool TryGetPitch(out PitchResult result)
        {
            return pitchQueue.TryDequeue(out result);
        }


        /*
         軸の設定
         */
        public void SetupXAxis(Chart chart)
        {
            var area = chart.ChartAreas[0];

            double start = songManager.PageStart;
            double end = songManager.PageEnd;

            double length = end - start;

            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = length;
            area.AxisX.Interval = length;
        }

        public void SetupYAxis(Chart chart)
        {
            var song = songManager.Song;
            int page = songManager.CurrentPage;

            var axisY = chart.ChartAreas[0].AxisY;
            axisY.CustomLabels.Clear();

            int centerNote = 60;
            int minNote = 48;
            int maxNote = 72;

            if (song != null && song.Pages[page].Notes.Count > 0)
            {
                var notes = song.Pages[page].Notes;

                minNote = notes.Min(n => n.MidiNote);
                maxNote = notes.Max(n => n.MidiNote);

                centerNote = (minNote + maxNote) / 2;

                int diff = maxNote - minNote;

                if (diff <= 22)
                {
                    minNote = centerNote - 12;
                    maxNote = centerNote + 12;
                }
                else
                {
                    minNote -= 1;
                    maxNote += 1;
                }
            }

            axisY.Minimum = minNote;
            axisY.Maximum = maxNote;
            axisY.Interval = 1;

            string[] noteNames =
            {
                "C","C#","D","D#","E","F",
                "F#","G","G#","A","A#","B"
            };

            for (int midi = minNote; midi <= maxNote; midi++)
            {
                string label = noteNames[midi % 12];
                axisY.CustomLabels.Add(midi - 0.5, midi + 0.5, label);
            }
        }
    }
}