using AndOS.Application.Components;
using Microsoft.AspNetCore.Components;

namespace AndOS.Application.Entities;

public class Program : AndOSBaseComponent
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public RenderFragment Icon { get; set; }
    public string Name { get; set; }
    public bool IsExternalProgram { get; set; }
    public bool CanMinimizeToTray { get; set; } // Minimizar para bandeja
    public bool InstantiateWindowOnOpen { get; set; }
    public bool AllowMultipleInstances { get; set; }
    public Assembly Assembly { get; set; }
    public Dictionary<string, object> Arguments { get; set; } = [];
    public Dictionary<string, object> DefaultArguments { get; set; } = [];
    public Dictionary<string, List<MenuItem>> MainMenuBarItems { get; set; } = [];

    public Program()
    {
        Assembly = Assembly.GetAssembly(GetType());
    }

    public RenderFragment ToRenderFragment() => builder =>
    {
        builder.OpenComponent(0, this.GetType());
        Dictionary<string, object> arguments = this.DefaultArguments ?? [];

        this.Arguments?.Select((value, index) => new
        {
            value,
            index
        })
                .ToList()
                .ForEach(x =>
                {
                    if (arguments.ContainsKey(x.value.Key))
                        arguments[x.value.Key] = x.value.Value;
                    else
                        arguments.Add(x.value.Key, x.value.Value);
                });

        arguments.Select((value, index) => new { value, index })
            .ToList()
            .ForEach(arg => builder.AddComponentParameter(arg.index, arg.value.Key, arg.value.Value));

        builder.CloseComponent();
    };
}