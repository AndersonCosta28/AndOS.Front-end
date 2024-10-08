﻿using AndOS.Module.FileExplorer.Enums;

namespace AndOS.Module.FileExplorer.Params.OpenFolder;

public class OpenFolderById : IOpenFolderParams
{
    public Guid Id { get; set; }

    public bool ClearNavigationHistory { get; set; }
    public bool ClearFowardHistory { get; set; }
    public NavigationType NavigationType { get; set; }

    public OpenFolderById(Guid id, NavigationType navigationType, bool clearNavigationHistory = false, bool clearFowardHistory = false)
    {
        this.Id = id;
        this.NavigationType = navigationType;
        this.ClearNavigationHistory = clearNavigationHistory;
        this.ClearFowardHistory = clearFowardHistory;
    }
}