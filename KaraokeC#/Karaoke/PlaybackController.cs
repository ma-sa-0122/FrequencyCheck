using System;

namespace Karaoke
{
    internal class PlaybackController
    {
        private readonly Action onPlay;
        private readonly Action onStop;
        private readonly Action onPageForward;
        private readonly Action onPageBack;

        public PlaybackController(Action onPlay, Action onStop, Action onPageForward, Action onPageBack)
        {
            this.onPlay = onPlay;
            this.onStop = onStop;
            this.onPageForward = onPageForward;
            this.onPageBack = onPageBack;
        }

        public void Play() => onPlay?.Invoke();
        public void Stop() => onStop?.Invoke();
        public void PageForward() => onPageForward?.Invoke();
        public void PageBack() => onPageBack?.Invoke();
    }
}
