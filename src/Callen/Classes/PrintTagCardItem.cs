using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Callen
{
    public class PrintTagCardItem : INotifyPropertyChanged
    {
        private bool isSelected;

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

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
