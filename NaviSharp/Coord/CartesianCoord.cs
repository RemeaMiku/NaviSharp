// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public partial record struct CartesianCoord : IFormattable, IEquatable<CartesianCoord>, IUnaryNegationOperators<CartesianCoord, CartesianCoord>, ISubtractionOperators<CartesianCoord, CartesianCoord, Vector<double>>, IAdditionOperators<CartesianCoord, Vector<double>, CartesianCoord>
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public readonly void Deconstruct(out double x, out double y, out double z)
    => (x, y, z) = (X, Y, Z);
    public CartesianCoord(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public CartesianCoord(params double[] xyz)
    {
        if (xyz.Length != 3)
            throw new ArgumentException("The array length must be 3");
        X = xyz[0];
        Y = xyz[1];
        Z = xyz[2];
    }

    public override readonly string ToString()
        => $"[X:{X},Y:{Y},Z:{Z}]";

    public readonly string ToString(string? format, IFormatProvider? formatProvider)
        => $"[X:{X.ToString(format, formatProvider)},Y:{Y.ToString(format, formatProvider)},Z:{Z.ToString(format, formatProvider)}]";

    public static Vector<double> operator -(CartesianCoord left, CartesianCoord right)
    => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static CartesianCoord operator +(CartesianCoord left, Vector<double> right)
    {
        if (!right.IsSizeOf(3))
            throw new ArgumentException("The vector should be three-dimensional.", nameof(right));
        return new(left.X + right.At(0), left.Y + right.At(1), left.Z + right.At(2));
    }

    public static CartesianCoord operator -(CartesianCoord value)
    => new(-value.X, -value.Y, -value.Z);

    public readonly double DistanceFrom(CartesianCoord other)
    => CalculateDistance(this, other);

    public readonly double DistanceFrom(double x, double y, double z)
    => CalculateDistance(X, Y, Z, x, y, z);

    public static double CalculateDistance(double x1, double y1, double z1, double x2, double y2, double z2)
    => Sqrt(Pow(x1 - x2, 2) + Pow(y1 - y2, 2) + Pow(z1 - z2, 2));

    public static double CalculateDistance(CartesianCoord left, CartesianCoord right)
    => CalculateDistance(left.X, left.Y, left.Z, right.X, right.Y, right.Z);
}