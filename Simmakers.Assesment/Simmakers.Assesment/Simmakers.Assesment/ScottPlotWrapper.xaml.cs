using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace Simmakers.Assesment
{
    /// <summary>
    /// Interaction logic for ScottPlotWrapper.xaml
    /// </summary>
    public partial class ScottPlotWrapper : UserControl
    {
        public ScottPlotWrapper()
        {
            InitializeComponent();
        }


        [Bindable(true)]
        public PlotData PlotData
        {
            get { return (PlotData)GetValue(PlotDataProperty); }
            set { SetValue(PlotDataProperty, value); }
        }

        public static readonly DependencyProperty PlotDataProperty =
            DependencyProperty.Register("PlotData", typeof(PlotData), typeof(ScottPlotWrapper), new PropertyMetadata(default(PlotData),
                new PropertyChangedCallback(OnDataHandler)));

        private static void OnDataHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScottPlotWrapper wrapper = (ScottPlotWrapper)d;

            PlotData data = (PlotData)e.NewValue;

            wrapper.WpfPlot1.Plot.AddScatter(data.DataX, data.DataY);
            wrapper.WpfPlot1.Refresh();
        }
    }
}
