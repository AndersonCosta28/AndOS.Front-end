using AndOS.Application.Components;
using AndOS.Module.FileExplorer.Components.File;
using AndOS.Module.FileExplorer.Components.Folder;
using AndOS.Module.FileExplorer.Enums;

namespace AndOS.Module.FileExplorer;

public class FileExplorerConfig
{
    public string FilesAccepts { get; set; } = "*";
    public ModeFileExplorer Mode { get; set; }
    public List<SystemItemBaseComponent> ItemsSelect { get; set; } = [];
    public ItemTypeForSelection ItemToSelect { get; set; } = ItemTypeForSelection.Both;
    public bool CanOpenFiles { get; set; } = false;

    public void OnSingleSelect(SystemItemBaseComponent component, bool value)
    {
        if (this.ItemToSelect == ItemTypeForSelection.Folder && !component.GetType().Equals(typeof(FolderItem)))
            return;

        if (this.ItemToSelect == ItemTypeForSelection.File && !component.GetType().Equals(typeof(FileItem)))
            return;

        var contains = this.ItemsSelect.Contains(component);
        if (!contains && value)
        {
            foreach (AndOSBaseComponent component2 in this.ItemsSelect)
                component2.Select(false);

            this.ItemsSelect.Clear();
            this.ItemsSelect.Add(component);
            component.Select(true);
        }
        else
        {
            foreach (var component2 in this.ItemsSelect.Where(componentInList => componentInList != component))
                component2.Select(false);

            this.ItemsSelect.RemoveAll(componentInList => componentInList != component);
            component.Select(true);
        }
    }

    public void OnMultipleSelect(SystemItemBaseComponent component, bool value)
    {
        if (this.ItemToSelect == ItemTypeForSelection.Folder && !component.GetType().Equals(typeof(FolderItem)))
            return;

        if (this.ItemToSelect == ItemTypeForSelection.File && !component.GetType().Equals(typeof(FileItem)))
            return;

        if (!this.ItemsSelect.Contains(component) && value)
        {
            this.ItemsSelect.Add(component);
            component.Select(true);
            return;
        }

        else if (this.ItemsSelect.Contains(component) && !value)
        {
            this.ItemsSelect.Remove(component);
            component.Select(false);
            return;
        }

        else
            component.Select(value);
    }
}