using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DockBar.DockItem.Helpers;

namespace DockBar.DockItem.Structs;

internal enum LinkType
{
    Exe,
    Lnk,
    Web,
    File,
    Folder,
}

internal sealed class DockLinkItem : IDockItem
{
    public required string Name { get; init; }

    public required string IconPath { get; init; }

    public required string LinkPath { get; init; }

    public required LinkType LinkType { get; init; }

    public void Start()
    {
        switch (LinkType)
        {
            case LinkType.Exe:
                Start(LinkPath, Path.GetDirectoryName(LinkPath));
                break;
            case LinkType.Lnk:
                Start(LinkPath);
                break;
            case LinkType.Web:
                Start(LinkPath);
                break;
            case LinkType.File:
                Start(LinkPath);
                break;
            case LinkType.Folder:
                Start(LinkPath);
                break;
            default:
                break;
        }
    }

    private static void Start(string pathOrUrl, string? workingDir = null)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo() { FileName = pathOrUrl, UseShellExecute = true, };
            if (workingDir is not null)
                processStartInfo.WorkingDirectory = workingDir;
            Process.Start(processStartInfo);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}
