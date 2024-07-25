using AndOS.Application.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AndOS.Application.Interfaces;

public interface IContextMenuManager
{
    public event Func<MouseEventArgs, Task> OnSetItems;
    public bool Open { get; set; }
    Task SetItems(IList<MenuItem> menus, MouseEventArgs args);
    IList<MenuItem> GetItems();
}