using System.Windows;
using System.Windows.Media;

namespace Callen
{
    public class PrintTagCardItem
    {
        public double TextBoxHeight { get; set; }
        public Brush BorderColor { get; set; }
        public Brush TextBoxBackground { get; set; }
        public Thickness TextBoxMargin { get; set; }

        public int ID { get; set; }
        public string Matrix { get; set; }
        public string Collec { get; set; }
        public string Year { get; set; }
        public string Name { get; set; }
        public int BreakSpan { get; set; } = 1;
    }
}
