using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace KailleraNET.Util
{
    public static class ColorUtil
    {

        /// <summary>
        /// Sets the random username color.  Edit constructor to limit, add, or change colors
        /// Note:  Light colors do not show well on white background
        /// </summary>
        public static Dictionary<int, SolidColorBrush> txtColors = new Dictionary<int, SolidColorBrush>();

        static ColorUtil()
        {
            txtColors.Add(0, Brushes.Black);
            txtColors.Add(1, Brushes.Blue);
            txtColors.Add(2, Brushes.Red);
            txtColors.Add(3, Brushes.Green);
            txtColors.Add(4, Brushes.MistyRose);
            txtColors.Add(5, Brushes.SpringGreen);
            txtColors.Add(6, Brushes.SandyBrown);
            txtColors.Add(7, Brushes.DarkCyan);
            txtColors.Add(8, Brushes.Purple);
            txtColors.Add(9, Brushes.Violet);
            txtColors.Add(10, Brushes.Orange);
            txtColors.Add(11, Brushes.RoyalBlue);
            txtColors.Add(12, Brushes.Olive);
            txtColors.Add(13, Brushes.Navy);
            txtColors.Add(14, Brushes.Firebrick);
            txtColors.Add(15, Brushes.DarkTurquoise);
            txtColors.Add(16, Brushes.DarkSalmon);
            txtColors.Add(17, Brushes.DeepPink);
            txtColors.Add(18, Brushes.OrangeRed);
            txtColors.Add(19, Brushes.Maroon);
            txtColors.Add(20, Brushes.Plum);
            txtColors.Add(21, Brushes.Tomato);
            txtColors.Add(22, Brushes.DarkViolet);
            txtColors.Add(23, Brushes.DarkGray);
            txtColors.Add(24, Brushes.Chocolate);
            txtColors.Add(25, Brushes.Aquamarine);
            txtColors.Add(26, Brushes.IndianRed);
            txtColors.Add(27, Brushes.MidnightBlue);
            txtColors.Add(28, Brushes.MediumSlateBlue);
        }
    }
}
