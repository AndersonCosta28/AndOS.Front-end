using Microsoft.AspNetCore.Components;

namespace AndOS.Application.Entities;

public class Window
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public int Index { get; set; } = 1;
    public bool Resize { get; set; }
    public bool Draggable { get; set; }
    public bool Focused { get; set; }
    public bool Hide { get; set; }
    public bool FullScreen { get; set; }
    public string Width { get; set; }
    public string Height { get; set; }
    public string Left { get; set; }
    public string Top { get; set; }

    public string WidthOnBeforeHide { get; set; }
    public string HeightOnBeforeHide { get; set; }
    public string LeftOnBeforeHide { get; set; }
    public string TopOnBeforeHide { get; set; }
    public bool WasHided { get; set; }
}