using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockBar.DockItem;

public interface IDockItemService
{
    IReadOnlyCollection<IDockItem> DockItems { get; }

    event EventHandler<DockItemChangedEventArgs>? DockItemChanged;

    void InsertDockLinkItem(string name, string linkPath, int index);

    void RemoveDockLinkItem(string name);

    void MoveDockLinkItem(string name, int index);

    void SaveData(string filePath);
    void ReadData(string filePath);
}

public class DockItemChangedEventArgs : EventArgs
{
    public required IDockItem DockItem { get; init; }
    public required bool IsAdd { get; init; }
}
