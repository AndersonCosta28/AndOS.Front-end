namespace AndOS.Application.Components;

public class MenuItem
{
    public string Label { get; }
    public Func<Task> OnClick { get; }
    public List<MenuItem> SubMenuItems { get; } = [];
    public bool Checked => CallBackChecked().Result;
    public bool Disable => CallBackDisabled().Result;
    public bool Visible => CallBackVisible().Result;
    private Func<Task<bool>> CallBackChecked { get; } = () => Task.FromResult(false);
    private Func<Task<bool>> CallBackDisabled { get; } = () => Task.FromResult(false);
    private Func<Task<bool>> CallBackVisible { get; } = () => Task.FromResult(true);

    public MenuItem(string label,
        Func<Task> onClick,
        Func<Task<bool>> callBackChecked = null,
        Func<Task<bool>> callBackDisabled = null,
        Func<Task<bool>> callBackVisible = null)
    {
        if (onClick is null)
            throw new Exception("Action cannot be null");
        Label = label;
        OnClick = onClick;

        if (callBackChecked is not null)
            CallBackChecked = callBackChecked;

        if (callBackDisabled is not null)
            CallBackDisabled = callBackDisabled;

        if (callBackVisible is not null)
            CallBackVisible = callBackVisible;
    }

    public MenuItem(string label, List<MenuItem> subMenuItems)
    {
        if (subMenuItems is null || subMenuItems.Count == 0)
            throw new Exception("You must have at least 1 item");

        Label = label;
        SubMenuItems = subMenuItems;
    }
}