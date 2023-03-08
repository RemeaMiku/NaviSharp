namespace NaviSharp;

public partial record struct CartCoord
{
    public GeoCoord ToGeo(EarthEllipsoid e)
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
        return new GeoCoord(B, L, H);
    }

    public static explicit operator double[](CartCoord coord)
    {
        var array = new double[3];
        array[0] = coord.X;
        array[1] = coord.Y;
        array[2] = coord.Z;
        return array;
    }

    public static explicit operator Vector<double>(CartCoord coord)
        => new(new double[] { coord.X, coord.Y, coord.Z });

    public Vector<double> ToVector() => new(new double[] { X, Y, Z });
    public static CartCoord FromGeo(GeoCoord coord, EarthEllipsoid e) => coord.ToCart(e);
    public static CartCoord FromVector(Vector<double> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("The vector has to be 3 dimensional", nameof(vector));
        return new(vector.At(0), vector.At(1), vector.At(2));
    }
}

public partial record struct GeoCoord
{
    public CartCoord ToCart(EarthEllipsoid e)
    {
        double n = e.N(Latitude);
        double temp = (n + Height) * Cos(Latitude);
        return new CartCoord(temp * Cos(Longitude), temp * Sin(Longitude), (n * (1 - e.E1 * e.E1) + Height) * Sin(Latitude));
    }
    public Vector<double> ToVector() => new(new double[] { Latitude.Rads, Longitude.Rads, Height });
    public static GeoCoord FromCart(CartCoord coord, EarthEllipsoid e) => coord.ToGeo(e);
    public static GeoCoord FromVector(Vector<double> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("The vector has to be 3 dimensional", nameof(vector));
        return new(vector.At(0), vector.At(1), vector.At(2));
    }

    public static explicit operator double[](GeoCoord coord)
        => new double[] { coord.Latitude.Rads, coord.Longitude.Rads, coord.Height };

    public static explicit operator Vector<double>(GeoCoord coord)
    => new(new double[] { coord.Latitude.Rads, coord.Longitude.Rads, coord.Height });
}