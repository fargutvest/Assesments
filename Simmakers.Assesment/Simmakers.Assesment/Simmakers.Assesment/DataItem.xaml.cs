using System.Windows;
using System.Windows.Controls;

namespace Simmakers.Assesment
{
    /// <summary>
    /// Interaction logic for DataItem.xaml
    /// </summary>
    public partial class DataItem : UserControl
    {
        private DataPointViewModel dataContext;

        public DataItem()
        {
            InitializeComponent();
        }

        public void OnGotFocusX(object sender, RoutedEventArgs e)
        {
            if (dataContext!= null)
            {
                dataContext.IsSelectedX = true;
            }
        }

        public void OnLostFocusX(object sender, RoutedEventArgs e)
        {
            if (dataContext != null)
            {
                dataContext.IsSelectedX = false;
            }
        }
        public void OnGotFocusY(object sender, RoutedEventArgs e)
        {
            if (dataContext != null)
            {
                dataContext.IsSelectedY = true;
            }
        }

        public void OnLostFocusY(object sender, RoutedEventArgs e)
        {
            if (dataContext != null)
            {
                dataContext.IsSelectedY = false;
            }
        }

        private void MyDataItem_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            dataContext = this.DataContext as DataPointViewModel;
        }
    }
}
