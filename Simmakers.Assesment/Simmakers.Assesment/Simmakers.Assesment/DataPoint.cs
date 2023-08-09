using System.ComponentModel;

namespace Simmakers.Assesment
{

    public class DataPoint : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double x;
        public double X 
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }

        private double y;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }

        public override string ToString()
        {
            return $"{X}; {Y}";
        }
    }
}
