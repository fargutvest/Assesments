using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AxonSoft.Assesment
{
    public static class ColorsHelper
    {
        public static Color[] GetShadesOfGreen(int countOfShades)
        {
            #region color parameters
            // these 4 parameters were found experimentally
            // they can be changed to achieve a better difference of shades by eye
            int minGreenHue = 120;
            int maxGreenHue = 140;
            double minBrightness = 0.1;
            double maxBrightness = 0.7;
            #endregion

            List<Color> allShadesOfGreen = new List<Color>();

            for (int r = 0; r < byte.MaxValue; r++)
            {
                for (int g = 0; g < byte.MaxValue; g ++)
                {
                    for (int b = 0; b < byte.MaxValue; b ++)
                    {
                        Color color = Color.FromArgb(r, g, b);
                        float hue = color.GetHue();
                        float brightness = color.GetBrightness();

                        if (hue > minGreenHue && hue < maxGreenHue && brightness > minBrightness && brightness < maxBrightness)
                        {
                            allShadesOfGreen.Add(color);
                        }
                    }
                }
            }

            var orderedShades = allShadesOfGreen.OrderByDescending(_ => _.GetBrightness() * _.GetHue() * _.GetSaturation()).ToArray();

            int samplingStep = Math.Abs(orderedShades.Length / countOfShades);

            var sampling = new List<Color>();
            for (int i = 0; i < orderedShades.Length; i += samplingStep)
            {
                sampling.Add(orderedShades[i]);
            }
            return sampling.ToArray();
        }
    }
}
