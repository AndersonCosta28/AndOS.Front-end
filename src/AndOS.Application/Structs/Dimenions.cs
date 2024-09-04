namespace AndOS.Application.Structs;

public struct Dimensions
{
    public Dimensions(decimal x, decimal y)
    {
        this.X = x;
        this.Y = y;
    }

    public decimal X { get; set; }
    public decimal Y { get; set; }
}