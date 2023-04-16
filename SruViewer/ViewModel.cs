namespace SruViewer;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

public sealed class ViewModel : INotifyPropertyChanged
{
    private readonly ObservableBatchCollection<SruItem> items = new();
    private int win;
    private int loss;

    public ViewModel()
    {
        this.Items = new(this.items);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReadOnlyObservableCollection<SruItem> Items { get; }

    public int Win
    {
        get => this.win;
        private set
        {
            if (value == this.win)
            {
                return;
            }

            this.win = value;
            this.OnPropertyChanged();
        }
    }

    public int Loss
    {
        get => this.loss;
        private set
        {
            if (value == this.loss)
            {
                return;
            }

            this.loss = value;
            this.OnPropertyChanged();
        }
    }

    public void Read(string fileName)
    {
        var net = 0;
        var sru = File.ReadAllText(fileName).AsSpan();
        var items = new List<SruItem>();
        while (SruItem.Read(ref sru) is { } item)
        {
            net += item.Win;
            net -= item.Loss;
            items.Add(item);
        }

        this.items.Reset(items);
        if (net > 0)
        {
            this.Win = net;
            this.Loss = 0;
        }
        else
        {
            this.Win = 0;
            this.Loss = -net;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
