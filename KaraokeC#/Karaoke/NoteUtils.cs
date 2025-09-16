using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Karaoke
{
    internal class NoteUtils
    {
        /// <summary>
        /// (曲名).json を読み込んで SongData を返す
        /// </summary>
        public static SongData getSongData(string songName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string songDir = Path.Combine(baseDir, "Songs");
            string jsonPath = Path.Combine(songDir, songName + ".json");

            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("JSONファイルが見つかりません: " + jsonPath);

            string json = File.ReadAllText(jsonPath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<SongData>(json);
        }

        /// <summary>
        /// Songsフォルダ内の曲リストを返す（拡張子なし）
        /// </summary>
        public static List<string> getMusicList()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string songDir = Path.Combine(baseDir, "Songs");

            if (!Directory.Exists(songDir))
                return new List<string>();

            return Directory.GetFiles(songDir, "*.json")
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToList();
        }

        public static string changeFIleNameToPath(string fileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string songDir = Path.Combine(baseDir, "Songs");
            string filepath = Path.Combine(songDir, fileName + ".mp3");

            if (!File.Exists(filepath))
                filepath = Path.Combine(songDir, fileName + ".wav");
            if (!File.Exists(filepath))
                throw new FileNotFoundException("楽曲ファイル形式が mp3 か wav ではありません。");

            return filepath;
        }
    }

    public class NoteEvent
    {
        public double StartSec { get; set; }   // 開始秒
        public double Duration { get; set; }   // 長さ秒
        public int MidiNote { get; set; }      // 60 = C4
        public string Color { get; set; }      // ノーツの色
    }

    public class PageInfo
    {
        public double StartSec { get; set; }   // ページ開始秒
        public double EndSec { get; set; }     // ページ終了秒
        public string lyrics { get; set; }     // 歌詞
        public List<NoteEvent> Notes { get; set; } = new List<NoteEvent>();
    }

    public class SongData
    {
        public int length { get; set; }        // 楽曲の長さ秒
        public List<PageInfo> Pages { get; set; } = new List<PageInfo>();
    }
}
