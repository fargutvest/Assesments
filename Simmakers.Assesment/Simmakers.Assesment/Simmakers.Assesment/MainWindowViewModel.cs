using Simmakers.Assesment.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Simmakers.Assesment
{
    class MainWindowViewModel 
    {
        public ObservableCollection<DataPointViewModel> Data { get; set; }

        public MainWindowViewModel()
        {
            Data = new ObservableCollection<DataPointViewModel>(new List<DataPointViewModel>() { GetNewDataPoint() });
        }

        private DataPointViewModel GetNewDataPoint(double x = 0, double y = 0)
        {
            DataPointViewModel dataPoint = new DataPointViewModel() { X = x, Y = y };
            dataPoint.Removed += DataPoint_Removed;
            return dataPoint;
        }

        private void DataPoint_Removed(DataPointViewModel toRemove)
        {
            Data.Remove(toRemove);
            if (Data.Count == 0)
            {
                Data.Add(GetNewDataPoint());
            }
        }

        private RelayCommand addNewDataPoint;
        public RelayCommand AddNewDataPoint
        {
            get
            {
                return addNewDataPoint ??
                  (addNewDataPoint = new RelayCommand(obj =>
                  {
                      Data.Add(GetNewDataPoint());
                  }));
            }
        }

        private RelayCommand cleanDataPoints;
        public RelayCommand CleanDataPoints
        {
            get
            {
                return cleanDataPoints ??
                  (cleanDataPoints = new RelayCommand(obj =>
                  {
                      Data.Clear();
                      Data.Add(GetNewDataPoint());
                  }));
            }
        }


        private RelayCommand toClipboard;
        public RelayCommand ToClipboard
        {
            get
            {
                return toClipboard ??
                  (toClipboard = new RelayCommand(obj =>
                  {
                      StringBuilder builder = new StringBuilder();
                      string tabSymbol = "	";
                      foreach (DataPointViewModel dataPoint in Data)
                      {
                          builder.AppendLine($"{dataPoint.X}{tabSymbol}{dataPoint.Y}");
                      }

                      Clipboard.SetText(builder.ToString());
                      MessageBox.Show(Resources.CopiedToClipboardMessage);
                  }));
            }
        }

        private RelayCommand fromClipboard;
        public RelayCommand FromClipboard
        {
            get
            {
                return fromClipboard ??
                  (fromClipboard = new RelayCommand(obj =>
                  {
                      try
                      {
                          string tabSymbol = "	";
                          string text = Clipboard.GetText();
                          string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                          List<DataPointViewModel> dataPoints = new List<DataPointViewModel>();
                          for (int i = 0; i < lines.Length; i++)
                          {
                              if (string.IsNullOrEmpty(lines[i]) == false)
                              {
                                  string[] splittedLine = lines[i].Split(new string[] { tabSymbol }, StringSplitOptions.None);
                                  string stringX = splittedLine[0].Replace(".", ",");
                                  string stringY = splittedLine[1].Replace(".", ",");
                                  DataPointViewModel dataPoint = GetNewDataPoint(double.Parse(stringX), double.Parse(stringY));
                                  dataPoints.Add(dataPoint);
                              }
                          }

                          Data.Clear();
                          foreach (var item in dataPoints)
                          {
                              Data.Add(item);
                          }
                      }
                      catch (Exception)
                      {
                          MessageBox.Show(Resources.WrongClipboardMessage, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                      }
                  }));
            }
        }

    }
}
