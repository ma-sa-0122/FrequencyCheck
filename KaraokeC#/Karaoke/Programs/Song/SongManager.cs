using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Song
{
    internal class SongManager
    {
        public SongData Song { get; private set; }

        public int CurrentPage { get; private set; }

        public double PageStart { get; private set; }
        public double PageEnd { get; private set; }

        public event Action<int> OnPageChanged;
        public event Action<string> OnLyricsChanged;

        private const double CONST_PAGE_SEC = 5.0;

        public void Load(SongData song)
        {
            Song = song;
            CurrentPage = 0;

            if (Song == null)
            {
                PageStart = 0;
                PageEnd = CONST_PAGE_SEC;

                OnPageChanged?.Invoke(CurrentPage);
                OnLyricsChanged?.Invoke("");
                return;
            }

            var p = Song.Pages[0];

            PageStart = p.StartSec;
            PageEnd = p.EndSec;

            OnPageChanged?.Invoke(CurrentPage);
            OnLyricsChanged?.Invoke(p.lyrics);
        }

        public void Update(double currentTime)
        {
            if (Song == null)
            {
                int newPage = (int)(currentTime / CONST_PAGE_SEC);

                if (newPage != CurrentPage)
                {
                    CurrentPage = newPage;
                    PageStart = newPage * CONST_PAGE_SEC;
                    PageEnd = PageStart + CONST_PAGE_SEC;

                    OnPageChanged?.Invoke(CurrentPage);
                    OnLyricsChanged?.Invoke("");
                }

                return;
            }

            while (CurrentPage < Song.Pages.Count &&
                   currentTime >= Song.Pages[CurrentPage].EndSec)
            {
                CurrentPage++;

                if (CurrentPage >= Song.Pages.Count)
                    return;

                var p = Song.Pages[CurrentPage];
                PageStart = p.StartSec;
                PageEnd = p.EndSec;

                OnPageChanged?.Invoke(CurrentPage);
                OnLyricsChanged?.Invoke(p.lyrics);
            }
        }

        public void ForcePage(int page)
        {
            if (Song == null) return;

            CurrentPage = page;

            var p = Song.Pages[page];
            PageStart = p.StartSec;
            PageEnd = p.EndSec;

            OnPageChanged?.Invoke(CurrentPage);
            OnLyricsChanged?.Invoke(p.lyrics);
        }

    }
}
