// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public partial record struct CartesianCoord
{
    public GeodeticCoord ToGeo(EarthEllipsoid e)
    {
        var L = Atan2(Y, X);
        var r = Sqrt(X * X + Y * Y);
        var zz = e.B * Abs(Z);
        var p = e.A * r;
        var a2 = e.A * e.A;
        var b2 = e.B * e.B;
        var q = a2 - b2;
        var add = 2 * (p + q);
        var sub = 2 * (p - q);
        var t = (zz + p + q) / (zz + 2 * p + q);
        var t2 = t * t;
        var dt = (t * (t2 * (t * zz + add) + sub) - zz) / (t2 * (4 * t * zz + 3 * add) + sub);
        while (dt > 1E-10)
        {
            t -= dt;
            t2 = t * t;
            dt = (t * (t2 * (t * zz + add) + sub) - zz) / (t2 * (4 * t * zz + 3 * add) + sub);
        }
        var B = 2 * (Sign(Z)) * (Atan(2 * e.A * t / (e.B * (1 - t2) + Sqrt(b2 * (1 - t2) * (1 - t2) + 4 * a2 * t2))));
        var H = (r * Cos(B) + Z * Sin(B) - e.A * e.W(B));
        return new GeodeticCoord(B, L, H);
    }

    public static explicit operator double[](CartesianCoord coord)
    {
        var array = new double[3];
        array[0] = coord.X;
        array[1] = coord.Y;
        array[2] = coord.Z;
        return array;
    }

    public static explicit operator Vector<double>(CartesianCoord coord)
        => new(new double[] { coord.X, coord.Y, coord.Z });

    public static explicit operator Vector3(CartesianCoord coord)
        => new((float)coord.X, (float)coord.Y, (float)coord.Z);
    public double[] ToArray() => (double[])this;
    public Vector<double> ToVector() => (Vector<double>)this;
    public Vector3 ToVector3() => (Vector3)this;
    public static CartesianCoord FromGeo(GeodeticCoord coord, EarthEllipsoid e) => coord.ToCart(e);
    public static CartesianCoord FromVector(Vector<double> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("The vector has to be 3 dimensional", nameof(vector));
        return new(vector.At(0), vector.At(1), vector.At(2));
    }
}

public partial record struct GeodeticCoord
{
    public CartesianCoord ToCart(EarthEllipsoid e)
    {
        double n = e.N(Latitude);
        double temp = (n + Altitude) * Cos(Latitude);
        return new CartesianCoord(temp * Cos(Longitude), temp * Sin(Longitude), (n * (1 - e.E1 * e.E1) + Altitude) * Sin(Latitude));
    }

    public double[] ToArray() => (double[])this;
    public Vector<double> ToVector() => new(new double[] { Latitude.Radians, Longitude.Radians, Altitude });
    public static GeodeticCoord FromCart(CartesianCoord coord, EarthEllipsoid e) => coord.ToGeo(e);
    public static GeodeticCoord FromVector(Vector<double> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("The vector has to be 3 dimensional", nameof(vector));
        return new(vector.At(0), vector.At(1), vector.At(2));
    }

    public static explicit operator double[](GeodeticCoord coord)
        => new double[] { coord.Latitude.Radians, coord.Longitude.Radians, coord.Altitude };

    public static explicit operator Vector<double>(GeodeticCoord coord)
    => new(new double[] { coord.Latitude.Radians, coord.Longitude.Radians, coord.Altitude });
}