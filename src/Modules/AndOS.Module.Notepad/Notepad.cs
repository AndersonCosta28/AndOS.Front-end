using AndOS.Application.Entities;

namespace AndOS.Module.Notepad;
public class Notepad : Program
{
    public Notepad() : base()
    {
        this.Name = nameof(Notepad);
        this.AllowMultipleInstances = true;
        this.Icon = builder =>
        {
            builder.OpenComponent<BlazorBootstrap.Icon>(0);
            builder.AddAttribute(1, "Name", BlazorBootstrap.IconName.JournalText);
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.CloseComponent();
        };
        this.Extensions = ["txt", "*"];
        this.ComponentType = typeof(NotepadComponent);
    }
}
