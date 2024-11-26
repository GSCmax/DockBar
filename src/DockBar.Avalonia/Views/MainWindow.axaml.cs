using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using DockBar.Avalonia.Controls;
using DockBar.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vanara.PInvoke;

namespace DockBar.Avalonia.Views;

public partial class MainWindow : Window
{
    internal MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        DockItemList.AddHandler(DragDrop.DropEvent, OnDrop);
        DockItemList.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        DockItemList.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);

        Closed += (s, e) =>
        {
            ViewModel.Global.SaveSettings();
        };
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ViewModel.Global.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GlobalViewModel.AutoPositionBottom))
            {
                TryMoveWindowToCenter();
            }
        };
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        ViewModel.IsDragMode = false;
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        ViewModel.IsDragMode = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        var index = PosXToIndex(e.GetPosition(DockItemList).X);
        foreach (var data in e.Data.GetFiles() ?? [])
        {
            ViewModel.InsertDockLinkItem(index, data.Path.LocalPath);
        }
        ViewModel.IsDragMode = false;
        ViewModel.Logger.Debug("OnDrop {Index}", index);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (ViewModel.IsMoveMode)
        {
            if (e.Pointer.Type is PointerType.Mouse && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
                e.Handled = true;
            }
        }

        ViewModel.SelectedDockLink = null;
        ViewModel.Logger.Debug("Window.OnPointerPressed");
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (ViewModel.IsMoveMode)
        {
            ViewModel.IsMoveMode = false;
            Cursor = Cursor.Default;
            e.Handled = true;
        }
    }

    private void CloseMenuItem_Clicked(object? sender, RoutedEventArgs e)
    {
        Close();
        e.Handled = true;
    }

    private void SettingMenuItem_Clicked(object? sender, RoutedEventArgs e)
    {
        var settingWindow = App.Instance.ServiceProvider.GetRequiredService<SettingWindow>();
        _ = settingWindow.ShowDialog(this);
        e.Handled = true;
        //if (_settingDialog is not null)
        //    return;
        //_settingDialog = new SettingDialog();
        //_settingDialog.Closed += (s, e) =>
        //{
        //    _settingDialog = null;
        //};
        //_settingDialog.ShowDialog(this);
        //e.Handled = true;
    }

    private void MoveMenuItem_Clicked(object? sender, RoutedEventArgs e)
    {
        Cursor = new Cursor(StandardCursorType.SizeAll);
        ViewModel.IsMoveMode = true;
    }

    private void AddLinkMenuItem_Clicked(object? sender, RoutedEventArgs e)
    {
        //if (_addDockItemDialog is not null)
        //    return;
        //_addDockItemDialog = new AddDockItemDialog();
        //_addDockItemDialog.Closed += (s, e) =>
        //{
        //    _addDockItemDialog = null;
        //};
        //_addDockItemDialog.ShowDialog(this);
        //e.Handled = true;
    }

    private void DockItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not DockItemControl source)
            return;

        ViewModel.SelectedDockLink = source.DockItem;
        ViewModel.Logger.Debug("DockItem.OnPointerPressed");
        e.Handled = true;
    }

    private void DeleteLinkMenuItem_Clicked(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedDockLink is null)
            return;
        ViewModel.RemoveDockLinkItem(ViewModel.SelectedDockLink);
        e.Handled = true;
    }

    int PosXToIndex(double x)
    {
        var curRight = GlobalViewModel.Instance.DockItemSize / 2;
        for (int i = 0; i < ViewModel.Links.Count; i++)
        {
            if (x <= curRight)
                return i;
            curRight += GlobalViewModel.Instance.DockItemSize + GlobalViewModel.Instance.DockItemSpacing;
        }
        return ViewModel.Links.Count;
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        TryMoveWindowToCenter();
    }

    private void TryMoveWindowToCenter()
    {
        if (ViewModel.Global.IsAutoPosition is true)
        {
            if (Screens.ScreenFromWindow(this) is Screen screen)
            {
                // 自动靠下居中
                var realWidth = Width * screen.Scaling;
                var realHeight = Height * screen.Scaling;

                var x = screen.Bounds.X + (screen.Bounds.Width - realWidth) / 2;
                var y = screen.Bounds.Y + screen.Bounds.Height - ViewModel.Global.AutoPositionBottom - realHeight;

                Position = new PixelPoint((int)x, (int)y);
            }
        }
    }
}
