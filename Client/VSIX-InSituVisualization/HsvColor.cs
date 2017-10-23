using System;
using System.Windows.Media;

namespace VSIX_InSituVisualization
{
    public class HsvColor
    {

        public HsvColor(double hue, double saturation, double value)
        {
            if (hue < 0 || hue > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(hue));
            }
            Hue = hue;

            if (saturation < 0 || saturation > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(saturation));
            }
            Saturation = saturation;

            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            Value = value;
        }

        public double Hue { get; }
        public double Saturation { get; }
        public double Value { get; }


        public static implicit operator Color(HsvColor hsvColor)
        {
            return hsvColor.ToColor();
        }

        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        private Color ToColor()
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            var h = Hue;
            while (h < 0) { h += 360; };
            while (h >= 360) { h -= 360; };
            double r, g, b;
            if (Value <= 0)
            { r = g = b = 0; }
            else if (Saturation <= 0)
            {
                r = g = b = Value;
            }
            else
            {
                double hf = h / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = Value * (1 - Saturation);
                double qv = Value * (1 - Saturation * f);
                double tv = Value * (1 - Saturation * (1 - f));
                switch (i)
                {

                    // Red is the dominant color
                    case 0:
                        r = Value;
                        g = tv;
                        b = pv;
                        break;
                    case 5:
                        r = Value;
                        g = pv;
                        b = qv;
                        break;

                    // Green is the dominant color
                    case 1:
                        r = qv;
                        g = Value;
                        b = pv;
                        break;
                    case 2:
                        r = pv;
                        g = Value;
                        b = tv;
                        break;

                    // Blue is the dominant color
                    case 3:
                        r = pv;
                        g = qv;
                        b = Value;
                        break;
                    case 4:
                        r = tv;
                        g = pv;
                        b = Value;
                        break;



                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        r = Value;
                        g = tv;
                        b = pv;
                        break;
                    case -1:
                        r = Value;
                        g = pv;
                        b = qv;
                        break;

                    // The color is not defined, we should throw an error.
                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        r = g = b = Value; // Just pretend its black/white
                        break;
                }
            }
            return Color.FromRgb((byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0));
        }
    }
}
