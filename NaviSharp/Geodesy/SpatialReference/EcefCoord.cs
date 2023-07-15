// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Collections;
using System.Numerics;

namespace NaviSharp.SpatialReference;

public readonly partial record struct EcefCoord :
    IEnumerable<double>,
    IFormattable,
    IEquatable<EcefCoord>,
    IUnaryNegationOperators<EcefCoord, EcefCoord>,
    ISubtractionOperators<EcefCoord, EcefCoord, Vector<double>>,
    IAdditionOperators<EcefCoord, Vector<double>, EcefCoord>
{
    private readonly double[] _data;
    public readonly double X
    {
        get => _data[0];
        set => _data[0] = value;
    }
    public readonly double Y
    {
        get => _data[1];
        set => _data[1] = value;
    }
    public readonly double Z
    {
        get => _data[2];
        set => _data[2] = value;
    }
    public readonly double[] Data => _data;
    public readonly void Deconstruct(out double x, out double y, out double z)
    => (x, y, z) = (X, Y, Z);
    public EcefCoord(double x, double y, double z)
    {
        _data = new double[] { x, y, z };
    }

    public EcefCoord(Vector<double> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("The vector has to be 3-dimensional.");
        _data = vector.Data;
    }

    public override readonly string ToString()
        => $"[X:{X},Y:{Y},Z:{Z}]";

    public readonly string ToString(string? format, IFormatProvider? formatProvider)
        => $"[X:{X.ToString(format, formatProvider)},Y:{Y.ToString(format, formatProvider)},Z:{Z.ToString(format, formatProvider)}]";

    public static Vector<double> operator -(EcefCoord left, EcefCoord right)
    => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static EcefCoord operator +(EcefCoord left, Vector<double> right)
    {
        if (!right.IsSizeOf(3))
            throw new ArgumentException("The vector should be three-dimensional.", nameof(right));
        return new(left.X + right.At(0), left.Y + right.At(1), left.Z + right.At(2));
    }

    public static EcefCoord operator -(EcefCoord value)
    => new(-value.X, -value.Y, -value.Z);

    public readonly double DistanceFrom(EcefCoord other)
    => DistanceBetween(this, other);

    public readonly double DistanceFrom(double x, double y, double z)
    => DistanceBetween(X, Y, Z, x, y, z);

    public static double DistanceBetween(double x1, double y1, double z1, double x2, double y2, double z2)
    => Sqrt(Pow(x1 - x2, 2) + Pow(y1 - y2, 2) + Pow(z1 - z2, 2));

    public static double DistanceBetween(EcefCoord left, EcefCoord right)
    => DistanceBetween(left.X, left.Y, left.Z, right.X, right.Y, right.Z);

    public readonly IEnumerator<double> GetEnumerator()
    {
        return ((IEnumerable<double>)_data).GetEnumerator();
    }

    readonly IEnumerator IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    public readonly Vector<double> ToVector() => new(_data);
    public readonly double[] ToArray() => _data;
    public GeodeticCoord ToGeodeticCoord(EarthEllipsoid earthEllipsoid)
    => CoordConverter.ToGeodetic(this, earthEllipsoid);
}