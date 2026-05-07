using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Karaoke
{
    internal class AudioInputService : IDisposable
    {
        private WasapiCapture capture;
        public event EventHandler<WaveInEventArgs> DataAvailable;

        public AudioInputService(MMDevice device, int samplingRate)
        {
            capture = new WasapiCapture(device);
            capture.WaveFormat = new WaveFormat(samplingRate, 16, 1);
            capture.DataAvailable += (s, e) => DataAvailable?.Invoke(this, e);
        }

        public void Start() => capture.StartRecording();
        public void Stop() => capture.StopRecording();
        public void Dispose()
        {
            if (capture != null) { capture.StopRecording(); capture.Dispose(); capture = null; }
        }
    }
}
