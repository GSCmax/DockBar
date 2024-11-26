using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using DockBar.DockItem;
using MessagePack;
using Serilog;

namespace DockBar.Avalonia.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModelBase
{
    const string StorageFile = ".dockItems";

    public IDockItemService DockItemService { get; }
    public ILogger Logger { get; }

    public ObservableCollection<IDockItem> Links { get; } = [];

    public GlobalViewModel Global { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectedDockLink))]
    private IDockItem? _selectedDockLink = null;

    public bool IsSelectedDockLink => SelectedDockLink != null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDockItemPanelEnabled))]
    private bool _isMoveMode = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDockItemPanelEnabled))]
    private bool _isDragMode = false;

    public bool IsDockItemPanelEnabled => !IsMoveMode && !IsDragMode;

    public double DockPanelWidth =>
        (Global.DockItemSize + Global.DockItemSpacing) * Links.Count
        + Global.DockItemExtendRate * Global.DockItemSize
        + Global.DockItemSpacing;

    public double DockPanelHeight => Global.DockItemSize * (1 + Global.DockItemExtendRate) + 8;

    public MainWindowViewModel(GlobalViewModel global, ILogger logger, IDockItemService dockItemService)
    {
        Global = global;
        Logger = logger;
        DockItemService = dockItemService;
        DockItemService.DockItemChanged += (s, e) =>
        {
            if (e.IsAdd)
            {
                Links.Add(e.DockItem);
            }
            else
            {
                Links.Remove(e.DockItem);
            }
        };
        DockItemService.ReadData(StorageFile);

        Global.PropertyChanged += (s, e) => NotifyPanelSize(this);
        DockItemService.DockItemChanged += (s, e) => NotifyPanelSize(this);
    }

    public void SaveDockLinkData()
    {
        DockItemService.SaveData(StorageFile);
    }

    public void InsertDockLinkItem(int index, string fullPath)
    {
        Logger.Debug($"AddDockLinkItem {fullPath}");
        DockItemService.InsertDockLinkItem(Path.GetFileNameWithoutExtension(fullPath), fullPath, index);
    }

    public void RemoveDockLinkItem(IDockItem item)
    {
        DockItemService.RemoveDockLinkItem(item.Name);
    }

    private static void NotifyPanelSize(MainWindowViewModel viewModel)
    {
        viewModel.OnPropertyChanged(nameof(DockPanelWidth));
        viewModel.OnPropertyChanged(nameof(DockPanelHeight));
    }
}
