namespace AndOS.Application.Structs;

public struct Dimensions
{
    public Dimensions(decimal x, decimal y)
    {
        X = x;
        Y = y;
    }

    public decimal X { get; set; }
    public decimal Y { get; set; }
}