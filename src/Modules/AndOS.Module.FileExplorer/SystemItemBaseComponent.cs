using AndOS.Application.Components;

namespace AndOS.Module.FileExplorer;

public class SystemItemBaseComponent : AndOSBaseComponent
{
    [CascadingParameter] public FileExplorerConfig FileExplorerConfig { get; set; }

    protected override void onClickOutSide(bool controlPressed, bool shiftPressed, MouseButton mouseButton) { }

    protected void LocalSelect(MouseEventArgs mouseEventArgs, bool select)
    {
        if (mouseEventArgs.CtrlKey)
            this.FileExplorerConfig.OnMultipleSelect(this, select);
        else
            this.FileExplorerConfig.OnSingleSelect(this, select);
    }

    protected void LocalSelect(bool select)
    {
        this.FileExplorerConfig.OnMultipleSelect(this, select);
    }
}