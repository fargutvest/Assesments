using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Simmakers.Assesment
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string labelX;
        public string LabelX
        {
            get
            {
                return labelX;
            }
            set
            {
                labelX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelX)));
            }
        }

        private string labelY;
        public string LabelY
        {
            get
            {
                return labelY;
            }
            set
            {
                labelY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelY)));
            }
        }

        public ObservableCollectionExtended<DataPoint> Data { get; set; }

        private Model model;

        public MainWindowViewModel()
        {
            model = new Model();

            LabelX = model.TempLabel;
            LabelY = model.AbsLabel;

            List<DataPoint> dataPoints = new List<DataPoint>();
            for (int i = 0; i < model.PointsX.Length; i++)
            {
                dataPoints.Add(new DataPoint() { X = model.PointsX[i], Y = model.PointsY[i] });
            }
            Data = new ObservableCollectionExtended<DataPoint>(dataPoints);
            Data.ChildChanged += OnChildChanged;
        }

        private void OnChildChanged(object sender, PropertyChangedEventArgs e)
        {
            Data = new ObservableCollectionExtended<DataPoint>(Data);
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
        }
    }
}
