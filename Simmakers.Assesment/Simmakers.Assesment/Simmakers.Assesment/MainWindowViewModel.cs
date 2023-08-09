using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Simmakers.Assesment
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string titleX;
        public string TitleX
        {
            get
            {
                return titleX;
            }
            set
            {
                titleX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleX)));
            }
        }

        private string titleY;
        public string TitleY
        {
            get
            {
                return titleY;
            }
            set
            {
                titleY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleY)));
            }
        }

        private PlotData data;
        public PlotData Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            }
        }

    

        public MainWindowViewModel()
        {
            TitleX = "Температура";
            TitleY = "Абсолютная отметка";



            List<double> points = new List<double>();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(i);
            }

            Data = new PlotData(points.ToArray(), points.ToArray());

        }
    }
}
