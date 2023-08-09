using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Simmakers.Assesment
{
    public partial class ChartWrapper : UserControl
    {
        public ChartWrapper()
        {
            InitializeComponent();
        }

        #region PlotData

        public IEnumerable<DataPoint> PlotData
        {
            get { return (IEnumerable<DataPoint>)GetValue(PlotDataProperty); }
            set { SetValue(PlotDataProperty, value); }
        }

        public static readonly DependencyProperty PlotDataProperty =
            DependencyProperty.Register("PlotData", typeof(IEnumerable<DataPoint>), typeof(ChartWrapper), 
                new PropertyMetadata(default(IEnumerable<DataPoint>),
                new PropertyChangedCallback(OnDataHandle)));

        private static void OnDataHandle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnHandle(d, wrapper =>
            {
                wrapper.WpfPlot1.Plot.Clear();
                DataPoint[] data = ((IEnumerable<DataPoint>)e.NewValue).ToArray();

                double[] dataX = new double[data.Length];
                double[] dataY = new double[data.Length];

                for (int i = 0; i < data.Length; i++)
                {
                    dataX[i] = data[i].X;
                    dataY[i] = data[i].Y;
                }

                wrapper.WpfPlot1.Plot.AddScatter(dataX, dataY);
            });
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
            wrapper.WpfPlot1.Refresh();
        }

    }
}
