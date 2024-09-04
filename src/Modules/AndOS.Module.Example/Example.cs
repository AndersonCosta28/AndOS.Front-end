using AndOS.Application.Entities;

namespace AndOS.Module.Example;

public class Example : Program
{
    public Example() : base()
    {
        this.Name = "Example";
        this.Icon = builder =>
        {
            builder.OpenComponent<BlazorBootstrap.Icon>(0);
            builder.AddAttribute(1, "Name", BlazorBootstrap.IconName.QuestionCircle); // Substitua pelo ícone desejado
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.CloseComponent();
        };

        this.ComponentType = typeof(ExampleComponent);
    }
}
