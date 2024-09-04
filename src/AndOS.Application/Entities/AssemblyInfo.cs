namespace AndOS.Application.Entities;

public class AssemblyInfo
{
    public AssemblyInfo() { }

    public AssemblyInfo(string name, Version version, byte[] binary)
    {
        this.Name = name;
        this.Version = version;
        this.Binary = binary;
    }

    public string Name { get; set; }
    public Version Version { get; set; }
    public byte[] Binary { get; set; }
}