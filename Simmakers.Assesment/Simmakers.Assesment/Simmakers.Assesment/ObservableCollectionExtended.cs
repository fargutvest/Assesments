using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Simmakers.Assesment
{
    public class ObservableCollectionExtended<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ChildChanged;

        public ObservableCollectionExtended(List<T> list) : base(list)
        {
            foreach (var item in list)
            {
                item.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChildChanged?.Invoke(sender, e);
        }

        public ObservableCollectionExtended(IEnumerable<T> collection) : base(collection) { }
    }
}
