using AndOS.Application.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace AndOS.Module.VideoPlayer;

public class VideoPlayer : Program
{
    public VideoPlayer() : base()
    {
        this.Name = "Video player";
        this.AllowMultipleInstances = false;
        this.Icon = builder =>
        {
            builder.OpenComponent<FluentIcon<Icons.Regular.Size20.VideoClip>>(0);
            builder.AddAttribute(1, "Value", new Icons.Regular.Size20.VideoClip());
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.AddAttribute(3, "Color", Color.Custom);
            builder.AddAttribute(4, "CustomColor", "#333");
            builder.CloseComponent();
        };
        this.Extensions = ["mp4", "webm", "ogg"];
        this.ComponentType = typeof(VideoPlayerComponent);
    }
}
