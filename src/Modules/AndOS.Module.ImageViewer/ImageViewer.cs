using AndOS.Application.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace AndOS.Module.ImageViewer;

public class ImageViewer : Program
{
    public ImageViewer() : base()
    {
        Name = "Imagem Viewer";
        AllowMultipleInstances = false;
        Icon = builder =>
        {
            builder.OpenComponent<FluentIcon<Icons.Regular.Size20.Image>>(0);
            builder.AddAttribute(1, "Value", new Icons.Regular.Size20.Image());
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.AddAttribute(3, "Color", Color.Custom);
            builder.AddAttribute(4, "CustomColor", "#333");
            builder.CloseComponent();
        };
        Extensions = ["jpg", "png", "gif", "svg", "webp"];
        ComponentType = typeof(ImageViewerComponent);
    }
}
