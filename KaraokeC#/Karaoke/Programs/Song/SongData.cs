using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Song
{
    internal class SongData
    {
        public int length { get; set; }        // 楽曲の長さ秒
        public List<PageInfo> Pages { get; set; } = new List<PageInfo>();
    }
}
