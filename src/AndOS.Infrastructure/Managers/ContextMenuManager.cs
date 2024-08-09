using AndOS.Application.Components;
using AndOS.Application.Interfaces;
using Microsoft.AspNetCore.Components.Web;

namespace AndOS.Infrastructure.Managers;

internal class ContextMenuManager : IContextMenuManager
{
    public bool Open { get; set; }
    IList<MenuItem> ContextMenuItems { get; set; } = [];

    public event Func<MouseEventArgs, Task> OnSetItems;

    public IList<MenuItem> GetItems() => ContextMenuItems;

    public async Task SetItems(IList<MenuItem> menus, MouseEventArgs args)
    {
        ContextMenuItems = menus;
        if (OnSetItems != null)
            await OnSetItems?.Invoke(args);
    }
}