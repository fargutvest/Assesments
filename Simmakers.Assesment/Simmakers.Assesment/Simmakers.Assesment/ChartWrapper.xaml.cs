using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace Simmakers.Assesment
{
    public partial class ChartWrapper : UserControl
    {
        public List<DataPointViewModel> DataPoints;

        public ChartWrapper()
        {
            InitializeComponent();
            DataPoints = new List<DataPointViewModel>();
        }

        #region PlotData

        public ObservableCollection<DataPointViewModel> PlotData
        {
            get { return (ObservableCollection<DataPointViewModel>)GetValue(PlotDataProperty); }
            set { SetValue(PlotDataProperty, value); }
        }

        public static readonly DependencyProperty PlotDataProperty =
            DependencyProperty.Register("PlotData", typeof(ObservableCollection<DataPointViewModel>), typeof(ChartWrapper), new PropertyMetadata(default(ObservableCollection<DataPointViewModel>),
                new PropertyChangedCallback(OnItemsChanged)));


        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnHandle(d, wrapper =>
            {
                var oldValue = e.OldValue as ObservableCollection<DataPointViewModel>;

                if (oldValue != null)
                {
                    oldValue.CollectionChanged -= wrapper.OnCollectionChanged;
                }

                var newValue = e.NewValue as ObservableCollection<DataPointViewModel>;

                if (newValue != null)
                {
                    wrapper.DataPoints.Clear();
                    wrapper.DataPoints.AddRange(newValue);
                    wrapper.RefreshChart();

                    newValue.CollectionChanged += wrapper.OnCollectionChanged;

                    foreach (var item in newValue)
                    {
                        item.PropertyChanged += wrapper.OnItemChanged;
                    }
                }
            });
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DataPoints.Clear();
                RefreshChart();
            }

            if (e.NewItems != null)
            {
                foreach (DataPointViewModel item in e.NewItems)
                {
                    DataPoints.Add(item);
                    RefreshChart();
                    item.PropertyChanged += OnItemChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (DataPointViewModel item in e.OldItems)
                {
                    DataPoints.Remove(item);
                    RefreshChart();
                    item.PropertyChanged -= OnItemChanged;
                }
            }
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshChart();
        }

        #endregion

        #region LabelX

        [Bindable(true)]
        public string LabelX
        {
            get { return (string)GetValue(LabelXProperty); }
            set { SetValue(LabelXProperty, value); }
        }

        public static readonly DependencyProperty LabelXProperty =
            DependencyProperty.Register("LabelX", typeof(string), typeof(ChartWrapper), new PropertyMetadata(default(string),
                new PropertyChangedCallback(OnLabelXHandle)));

        private static void OnLabelXHandle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnHandle(d, wrapper =>
            {
                string data = (string)e.NewValue;
                wrapper.WpfPlot1.Plot.XLabel(data);
            });
        }

        #endregion


        #region LabelY

        [Bindable(true)]
        public string LabelY
        {
            get { return (string)GetValue(LabelYProperty); }
            set { SetValue(LabelYProperty, value); }
        }

        public static readonly DependencyProperty LabelYProperty =
            DependencyProperty.Register("LabelY", typeof(string), typeof(ChartWrapper), new PropertyMetadata(default(string),
                new PropertyChangedCallback(OnLabelYHandle)));

        private static void OnLabelYHandle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnHandle(d, wrapper =>
            {
                string data = (string)e.NewValue;
                wrapper.WpfPlot1.Plot.YLabel(data);
            });
        }

        #endregion

        private static void OnHandle(DependencyObject d, Action<ChartWrapper> toDo)
        {
            ChartWrapper wrapper = (ChartWrapper)d;
            toDo?.Invoke(wrapper);
        }

        private void RefreshChart()
        {
            if (DataPoints.Count > 0)
            {
                WpfPlot1.Plot.Clear();

                double[] dataX = new double[DataPoints.Count];
                double[] dataY = new double[DataPoints.Count];

                for (int i = 0; i < DataPoints.Count; i++)
                {
                    dataX[i] = DataPoints[i].X;
                    dataY[i] = DataPoints[i].Y;
                }

                WpfPlot1.Plot.AddScatter(dataX, dataY);
                WpfPlot1.Refresh();
            }
        }

    }
}
