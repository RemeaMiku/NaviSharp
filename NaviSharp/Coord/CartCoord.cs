namespace NaviSharp;

public partial record struct CartCoord : IFormattable, IEquatable<CartCoord>
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public CartCoord(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public CartCoord(double[] xyz)
    {
        if (xyz.Length != 3)
            throw new ArgumentException("The array length must be 3");
        X = xyz[0];
        Y = xyz[1];
        Z = xyz[2];
    }

    public override string ToString()
        => $"[X:{X},Y:{Y},Z:{Z}]";

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"[X:{X.ToString(format, formatProvider)},Y:{Y.ToString(format, formatProvider)},Z:{Z.ToString(format, formatProvider)}]";
}