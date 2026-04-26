using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Audio
{
    internal class AudioPlayer
    {
        private WasapiOut output;
        private AudioFileReader reader;

        public bool IsPlaying => output != null;

        public void Load(string path, int latencyMs)
        {
            Dispose();

            reader = new AudioFileReader(path);
            output = new WasapiOut(AudioClientShareMode.Shared, true, latencyMs);
            output.Init(reader);
        }

        public void Play()
        {
            output?.Play();
        }

        public void Stop()
        {
            output?.Stop();
        }

        public void SetVolume(float volume)
        {
            if (reader != null)
                reader.Volume = volume;
        }

        public double CurrentTime =>
            reader?.CurrentTime.TotalSeconds ?? 0;

        public double Length =>
            reader?.TotalTime.TotalSeconds ?? 0;

        public void Seek(double sec)
        {
            if (reader != null)
                reader.CurrentTime = TimeSpan.FromSeconds(sec);
        }

        public void Dispose()
        {
            output?.Dispose();
            reader?.Dispose();
            output = null;
            reader = null;
        }
    }
}
