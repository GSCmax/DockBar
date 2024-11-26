using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockBar.Avalonia.ViewModels;

internal sealed class SettingWindowViewModel : ViewModelBase
{
    public GlobalViewModel Global { get; }

    public SettingWindowViewModel(GlobalViewModel global)
    {
        Global = global;
    }
}
