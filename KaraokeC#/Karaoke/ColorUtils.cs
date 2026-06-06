using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke
{
    internal class ColorUtils
    {
        public static Color ParseColor(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Color.White;

            value = value.Trim();

            // #RRGGBB
            // #AARRGGBB
            if (value.StartsWith("#"))
            {
                value = value.Substring(1);

                // RRGGBB
                if (value.Length == 6)
                {
                    int r = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber);
                    int g = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber);
                    int b = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber);

                    return Color.FromArgb(r, g, b);
                }

                // AARRGGBB
                if (value.Length == 8)
                {
                    int a = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber);
                    int r = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber);
                    int g = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber);
                    int b = int.Parse(value.Substring(6, 2), NumberStyles.HexNumber);

                    return Color.FromArgb(a, r, g, b);
                }
            }

            // "255,0,128"
            if (value.Contains(","))
            {
                string[] parts = value.Split(',');

                if (parts.Length == 3)
                {
                    int r = int.Parse(parts[0]);
                    int g = int.Parse(parts[1]);
                    int b = int.Parse(parts[2]);

                    return Color.FromArgb(r, g, b);
                }

                // "128,255,0,128"
                if (parts.Length == 4)
                {
                    int a = int.Parse(parts[0]);
                    int r = int.Parse(parts[1]);
                    int g = int.Parse(parts[2]);
                    int b = int.Parse(parts[3]);

                    return Color.FromArgb(a, r, g, b);
                }
            }

            // 色名
            return Color.FromName(value);
        }
    }
}
