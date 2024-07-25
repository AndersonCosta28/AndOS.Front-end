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

    public void OnSingleSelect(SystemItemBaseComponent component, bool value)
    {
        if (ItemToSelect == ItemTypeForSelection.Folder && !component.GetType().Equals(typeof(FolderItemComponent)))
            return;

        if (ItemToSelect == ItemTypeForSelection.File && !component.GetType().Equals(typeof(FileItemComponent)))
            return;

        bool contains = ItemsSelect.Contains(component);
        if (!contains && value)
        {
            foreach (AndOSBaseComponent component2 in ItemsSelect)
                component2.Select(false);

            ItemsSelect.Clear();
            ItemsSelect.Add(component);
            component.Select(true);
        }
        else
        {
            foreach (SystemItemBaseComponent component2 in ItemsSelect.Where(componentInList => componentInList != component))
                component2.Select(false);

            ItemsSelect.RemoveAll(componentInList => componentInList != component);
            component.Select(true);
        }
    }

    public void OnMultipleSelect(SystemItemBaseComponent component, bool value)
    {
        if (ItemToSelect == ItemTypeForSelection.Folder && !component.GetType().Equals(typeof(FolderItemComponent)))
            return;

        if (ItemToSelect == ItemTypeForSelection.File && !component.GetType().Equals(typeof(FileItemComponent)))
            return;

        if (!ItemsSelect.Contains(component) && value)
        {
            ItemsSelect.Add(component);
            component.Select(true);
            return;
        }

        else if (ItemsSelect.Contains(component) && !value)
        {
            ItemsSelect.Remove(component);
            component.Select(false);
            return;
        }

        else
            component.Select(value);
    }
}