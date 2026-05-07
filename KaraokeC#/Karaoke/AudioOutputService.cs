using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Karaoke
{
    internal class AudioOutputService : IDisposable
    {
        private WasapiOut output;
        public AudioOutputService(MMDevice device, double latencySeconds)
        {
            output = new WasapiOut(device, AudioClientShareMode.Shared, true, (int)(latencySeconds * 1000));
        }

        public void Init(AudioFileReader music) => output.Init(music);
        public void Play() => output.Play();
        public void Pause() => output.Pause();
        public void Dispose()
        {
            output?.Stop();
            output?.Dispose();
            output = null;
        }
    }
}
