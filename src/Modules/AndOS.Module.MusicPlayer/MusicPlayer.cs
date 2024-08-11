using AndOS.Application.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace AndOS.Module.MusicPlayer;

public class MusicPlayer : Program
{
    public MusicPlayer() : base()
    {
        Name = "Music Player";
        AllowMultipleInstances = false;
        Icon = builder =>
        {
            builder.OpenComponent<FluentIcon<Icons.Regular.Size20.MusicNote1>>(0);
            builder.AddAttribute(1, "Value", new Icons.Regular.Size20.MusicNote1()); // Substitua pelo ícone desejado
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.AddAttribute(3, "Color", Color.Custom);
            builder.AddAttribute(4, "CustomColor", "#333");
            builder.CloseComponent();
        };
        ComponentType = typeof(MusicPlayerComponent);
    }
}
