using AndOS.Application.Components;
using Microsoft.AspNetCore.Components;

namespace AndOS.Application.Entities;

public class Program : AndOSBaseComponent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RenderFragment Icon { get; set; }
    public string Name { get; set; }
    public bool IsExternalProgram { get; set; }
    public bool CanMinimizeToTray { get; set; }
    public bool InstantiateWindowOnOpen { get; set; } = true;
    public bool AllowMultipleInstances { get; set; }
    public Assembly Assembly { get; set; }
    [Parameter] public Dictionary<string, object> Arguments { get; set; } = [];
    public Dictionary<string, object> DefaultArguments { get; set; } = [];
    public Dictionary<string, List<MenuItem>> MainMenuBarItems { get; set; } = [];
    public List<string> Extensions { get; set; } = [];
    public Program()
    {
        this.Assembly = Assembly.GetAssembly(GetType());
    }

    public RenderFragment ToRenderFragment() => builder =>
    {
        builder.OpenComponent(0, this.GetType());
        var arguments = this.DefaultArguments ?? [];

        this.Arguments?.Select((keyValuePair, index) => new
        {
            keyValuePair,
            index
        })
            .ToList()
            .ForEach(x => arguments[x.keyValuePair.Key] = x.keyValuePair.Value);
        builder.AddAttribute(1, nameof(Arguments), arguments);
        builder.CloseComponent();
    };
}