using AndOS.Application.Entities;

namespace AndOS.Module.UserConfiguration;
public class UserConfiguration : Program
{
    public UserConfiguration() : base()
    {
        Name = "User configuration";
        AllowMultipleInstances = false;
        Icon = builder =>
        {
            builder.OpenComponent<BlazorBootstrap.Icon>(0);
            builder.AddAttribute(1, "Name", BlazorBootstrap.IconName.PersonFillGear); // Substitua pelo ícone desejado
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.CloseComponent();
        };
        ComponentType = typeof(UserConfigurationComponent);
    }
}
