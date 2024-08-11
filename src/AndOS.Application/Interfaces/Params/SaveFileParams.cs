namespace AndOS.Application.Interfaces.Params;
public record SaveFileParams(Guid? Id, string Name, string Extension, Guid ParentFolderId, Core.Enums.CloudStorage CloudStorage, string Content);
