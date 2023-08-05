using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AxonSoft.Assesment
{
    public static class ColorsHelper
    {
        public static Color[] GetAllShadesOfGreen()
        {
            List<Color> allShadesOfGreen = new List<Color>();

            int length = 255;
            int step = 5;
            for (int r = 0; r < length; r += step)
            {
                for (int g = 0; g < length; g += step)
                {
                    for (int b = 0; b < length; b += step)
                    {
                        Color color = Color.FromArgb(r, g, b);
                        float hue = color.GetHue();
                        float brightness = color.GetBrightness();
                        float saturation = color.GetSaturation();

                        if (hue > 100 && hue < 150 && brightness > 0.1 && brightness < 0.8)
                        {
                            allShadesOfGreen.Add(color);
                        }
                    }
                }
            }

            var ordered = allShadesOfGreen.OrderByDescending(_ => _.GetBrightness()).ToArray();

            var result = new List<Color>();
            for (int i = 0; i < ordered.Length; i += 100)
            {
                result.Add(ordered[i]);
            }


            return result.ToArray();
        }
    }
}
