// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp.SpatialReference;

public static class CoordConverter
{
    #region Public Methods

    public static EcefCoord ToEcef(GeodeticCoord geodeticCoord, EarthEllipsoid earthEllipsoid)
    {
        var n = earthEllipsoid.N(geodeticCoord.L);
        var temp = (n + geodeticCoord.H) * Cos(geodeticCoord.Latitude);
        return new EcefCoord(temp * Cos(geodeticCoord.L), temp * Sin(geodeticCoord.L), (n * (1 - earthEllipsoid.E1 * earthEllipsoid.E1) + geodeticCoord.H) * Sin(geodeticCoord.B));
    }

    public static GeodeticCoord ToGeodetic(EcefCoord ecefCoord, EarthEllipsoid earthEllipsoid)
    {
        var L = Atan2(ecefCoord.Y, ecefCoord.X);
        var r = Sqrt(ecefCoord.X * ecefCoord.X + ecefCoord.Y * ecefCoord.Y);
        var zz = earthEllipsoid.B * Abs(ecefCoord.Z);
        var p = earthEllipsoid.A * r;
        var a2 = earthEllipsoid.A * earthEllipsoid.A;
        var b2 = earthEllipsoid.B * earthEllipsoid.B;
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
        var B = 2 * (Sign(ecefCoord.Z)) * (Atan(2 * earthEllipsoid.A * t / (earthEllipsoid.B * (1 - t2) + Sqrt(b2 * (1 - t2) * (1 - t2) + 4 * a2 * t2))));
        var H = (r * Cos(B) + ecefCoord.Z * Sin(B) - earthEllipsoid.A * earthEllipsoid.W(B));
        return new GeodeticCoord(B, L, H);
    }

    #endregion Public Methods
}

//public partial record struct EcefCoord
//{
//    public readonly GeodeticCoord ToGeo(EarthEllipsoid e)
//    {
//        var L = Atan2(Y, X);
//        var r = Sqrt(X * X + Y * Y);
//        var zz = e.B * Abs(Z);
//        var p = e.A * r;
//        var a2 = e.A * e.A;
//        var b2 = e.B * e.B;
//        var q = a2 - b2;
//        var add = 2 * (p + q);
//        var sub = 2 * (p - q);
//        var t = (zz + p + q) / (zz + 2 * p + q);
//        var t2 = t * t;
//        var dt = (t * (t2 * (t * zz + add) + sub) - zz) / (t2 * (4 * t * zz + 3 * add) + sub);
//        while (dt > 1E-10)
//        {
//            t -= dt;
//            t2 = t * t;
//            dt = (t * (t2 * (t * zz + add) + sub) - zz) / (t2 * (4 * t * zz + 3 * add) + sub);
//        }
//        var B = 2 * (Sign(Z)) * (Atan(2 * e.A * t / (e.B * (1 - t2) + Sqrt(b2 * (1 - t2) * (1 - t2) + 4 * a2 * t2))));
//        var H = (r * Cos(B) + Z * Sin(B) - e.A * e.W(B));
//        return new GeodeticCoord(B, L, H);
//    }

//    public static explicit operator double[](EcefCoord coord)
//    {
//        var array = new double[3];
//        array[0] = coord.X;
//        array[1] = coord.Y;
//        array[2] = coord.Z;
//        return array;
//    }

//    public static explicit operator Vector<double>(EcefCoord coord)
//        => new(coord.X, coord.Y, coord.Z);

//    public static explicit operator Vector3(EcefCoord coord)
//        => new((float)coord.X, (float)coord.Y, (float)coord.Z);
//    public readonly double[] ToArray() => (double[])this;
//    public readonly Vector<double> ToVector() => (Vector<double>)this;
//    public readonly Vector3 ToVector3() => (Vector3)this;
//    public static EcefCoord FromGeo(GeodeticCoord coord, EarthEllipsoid e) => coord.ToCart(e);
//    public static EcefCoord FromVector(Vector<double> vector)
//    {
//        if (!vector.IsSizeOf(3))
//            throw new ArgumentException("The vector has to be 3 dimensional", nameof(vector));
//        return new(vector.At(0), vector.At(1), vector.At(2));
//    }
//}

//public partial record struct GeodeticCoord
//{
//    public EcefCoord ToCart(EarthEllipsoid e)
//    {
//        double n = e.N(Latitude);
//        double temp = (n + Altitude) * Cos(Latitude);
//        return new EcefCoord(temp * Cos(Longitude), temp * Sin(Longitude), (n * (1 - e.E1 * e.E1) + Altitude) * Sin(Latitude));
//    }

//    public double[] ToArray() => (double[])this;
//    public Vector<double> ToVector() => new(Latitude.Radians, Longitude.Radians, Altitude);
//    public static GeodeticCoord FromCart(EcefCoord coord, EarthEllipsoid e) => coord.ToGeo(e);
//    public static GeodeticCoord FromVector(Vector<double> vector)
//    {
//        if (!vector.IsSizeOf(3))
//            throw new ArgumentException("The vector has to be 3 dimensional", nameof(vector));
//        return new(vector.At(0), vector.At(1), vector.At(2));
//    }

//    public static explicit operator double[](GeodeticCoord coord)
//        => new double[] { coord.Latitude.Radians, coord.Longitude.Radians, coord.Altitude };

//    public static explicit operator Vector<double>(GeodeticCoord coord)
//    => new(coord.Latitude.Radians, coord.Longitude.Radians, coord.Altitude);
//}