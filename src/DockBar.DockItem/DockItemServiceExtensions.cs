using DockBar.DockItem.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace DockBar.DockItem;

public static class DockItemServiceExtensions
{
    public static IServiceCollection UseDockItemService(this IServiceCollection collection)
    {
        collection.AddSingleton<IDockItemService, DockItemService>();
        return collection;
    }
}
