using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProcessorHeatmap
{
    class ColourScheme
    {
        private static ColourScheme[] _schemes;

        public string Name { get; set; }
        public Color DisplayColour { get; set; }
        public Func<double, Color> ColourTranslation { get; set; }

        public ColourScheme(string name, Color displayColour, Func<double, Color> translation)
        {
            Name = name;
            DisplayColour = displayColour;
            ColourTranslation = translation;
        }

        public override string ToString()
        {
            return Name;
        }

        public Color GetColour(double percent)
        {
            return ColourTranslation(percent);
        }

        public static ColourScheme[] Schemes
        {
            get
            {
                if (_schemes == null)
                {
                    _schemes = new ColourScheme[]
                    {
                        new ColourScheme("Red", Colors.Red, (double percent) =>
                        {
                            byte r = (byte)((1.0f - (percent / 100.0f)) * 255.0f);
                            return Color.FromRgb(255, r, r);
                        }),
                        new ColourScheme("Green", Colors.Green, (double percent) =>
                        {
                            byte g = (byte)((1.0f - (percent / 100.0f)) * 255.0f);
                            return Color.FromRgb(g, 255, g);
                        }),
                        new ColourScheme("Blue", Colors.Blue, (double percent) =>
                        {
                            byte b = (byte)((1.0f - (percent / 100.0f)) * 255.0f);
                            return Color.FromRgb(b, b, 255);
                        }),
                        new ColourScheme("Pink", Colors.Purple, (double percent) =>
                        {
                            byte x = (byte)((1.0f - (percent / 100.0f)) * 255.0f);
                            return Color.FromRgb(255, x, 255);
                        }),
                    };
                }
                return _schemes;
            }
        }
    }
}
