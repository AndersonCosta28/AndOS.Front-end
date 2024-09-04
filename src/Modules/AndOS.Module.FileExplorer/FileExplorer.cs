using AndOS.Application.Entities;

namespace AndOS.Module.FileExplorer;
public class FileExplorer : Program
{
    public FileExplorer() : base()
    {
        this.Name = "File explorer";
        this.AllowMultipleInstances = true;
        this.Icon = builder =>
        {
            builder.OpenComponent<FluentIcon<Icons.Regular.Size20.FolderSearch>>(0);
            builder.AddAttribute(1, "Value", new Icons.Regular.Size20.FolderSearch()); // Substitua pelo ícone desejado
            builder.AddAttribute(2, "style", "width: inherit; font-size: inherit;");
            builder.AddAttribute(3, "Color", Color.Custom);
            builder.AddAttribute(4, "CustomColor", "#333");
            builder.CloseComponent();
        };
        this.ComponentType = typeof(FileExplorerComponent);
    }
}
