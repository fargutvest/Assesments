using System;
using System.ComponentModel;

namespace Simmakers.Assesment
{
    public class DataPointViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<DataPointViewModel> Removed;

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

        private bool isSelectedX;
        public bool IsSelectedX
        {
            get
            {
                return isSelectedX;
            }
            set
            {
                isSelectedX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedX)));
            }
        }

        private bool isSelectedY;
        public bool IsSelectedY 
        {
            get
            {
                return isSelectedY;
            }
            set
            {
                isSelectedY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedY)));
            }
        }

        public Guid Id { get; private set; }

        public DataPointViewModel()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"{X}; {Y}";
        }

        
        private RelayCommand removeDataPoint;
        public RelayCommand RemoveDataPoint
        {
            get
            {
                return removeDataPoint ??
                  (removeDataPoint = new RelayCommand(obj =>
                  {
                      Removed?.Invoke(this);
                  }));
            }
        }
    }
}
