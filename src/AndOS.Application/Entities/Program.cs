using Microsoft.AspNetCore.Components;

namespace AndOS.Application.Entities;

public class Program
{
    public Program() => this.Id = Guid.NewGuid();

    public Guid Id { get; init; }
    public RenderFragment Icon { get; init; }
    public string Name { get; init; }
    public bool IsExternalProgram { get; set; }
    public bool CanMinimizeToTray { get; init; }
    public bool InstantiateWindowOnOpen { get; init; } = true;
    public bool AllowMultipleInstances { get; init; }
    public Assembly Assembly => Assembly.GetAssembly(this.GetType());
    public Dictionary<string, object> DefaultArguments { get; init; } = [];
    public List<string> Extensions { get; init; } = [];
    public Type ComponentType { get; init; }

    public RenderFragment ToRenderFragment() => builder =>
    {
        builder.OpenComponent(0, this.ComponentType);
        var arguments = this.DefaultArguments ?? [];
        builder.AddAttribute(2, nameof(Program), this);
        builder.AddAttribute(2, "Arguments", arguments);
        builder.CloseComponent();
    };
}