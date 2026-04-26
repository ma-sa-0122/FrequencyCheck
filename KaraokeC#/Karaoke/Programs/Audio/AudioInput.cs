using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Audio
{
    internal class AudioInput
    {
        private WasapiCapture capture;
        private float[] buffer;
        private int bufferSize;
        private int index;

        public event Action<double[]> OnFrame;

        public AudioInput(int frameSize = 2048)
        {
            bufferSize = frameSize;
            buffer = new float[frameSize];
        }

        public void Start()
        {
            capture = new WasapiCapture();
            capture.DataAvailable += OnDataAvailable;
            capture.StartRecording();
        }

        public void Stop()
        {
            capture?.StopRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            int samples = e.BytesRecorded / 4;

            for (int i = 0; i < samples; i++)
            {
                float sample = BitConverter.ToSingle(e.Buffer, i * 4);

                buffer[index++] = sample;

                if (index >= bufferSize)
                {
                    var frame = new double[bufferSize];
                    for (int j = 0; j < bufferSize; j++)
                        frame[j] = buffer[j];

                    OnFrame?.Invoke(frame);
                    index = 0;
                }
            }
        }
    }
}
