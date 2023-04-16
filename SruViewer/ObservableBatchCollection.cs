namespace SruViewer;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

public sealed class ObservableBatchCollection<T> : ObservableCollection<T>
{
    private static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
    private static readonly PropertyChangedEventArgs IndexerPropertyChanged = new(Binding.IndexerName);
    private static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            this.Items.Add(item);
        }

        this.OnPropertyChanged(CountPropertyChanged);
        this.OnPropertyChanged(IndexerPropertyChanged);
        this.OnCollectionChanged(ResetCollectionChanged);
    }

    public void Reset(IEnumerable<T> items)
    {
        this.Items.Clear();
        this.AddRange(items);
    }
}
