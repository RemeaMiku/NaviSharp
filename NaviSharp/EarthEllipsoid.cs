// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public readonly partial struct EarthEllipsoid : IEquatable<EarthEllipsoid>, IEqualityOperators<EarthEllipsoid, EarthEllipsoid, bool>
{
    #region Public Constructors

    static EarthEllipsoid()
    {
        Wgs84 = FromAWithB(Wgs84Constants.a, Wgs84Constants.b, "Wgs84");
        Cgcs2000 = FromAWithB(Cgcs2000Constants.a, Cgcs2000Constants.b, "Cgcs2000");
        Grs80 = FromAWithB(Grs80Constants.a, Grs80Constants.b, "Grs80");
    }

    public EarthEllipsoid(double a, double b, string name = "UnnamedEllipsoid")
    {
        Name = name;
        EquatorialRadius = a;
        PolarRadius = b;
        var e = GetEccentricity(a, b);
        FirstEccentricity = e.E1;
        SecondEccentricity = e.E2;
        Oblateness = GetOblateness(a, b);
    }

    #endregion Public Constructors

    #region Public Properties

    public static EarthEllipsoid Cgcs2000 { get; }
    public static EarthEllipsoid Grs80 { get; }
    public static EarthEllipsoid Wgs84 { get; }
    public double A { get => EquatorialRadius; }
    public double Alpha { get => Oblateness; }
    public double B { get => PolarRadius; }
    public double E1 { get => FirstEccentricity; }
    public double E2 { get => SecondEccentricity; }
    public double EquatorialRadius { get; private init; }
    public double FirstEccentricity { get; private init; }
    public string Name { get; private init; }
    public double Oblateness { get; private init; }
    public double PolarRadius { get; private init; }
    public double SecondEccentricity { get; private init; }

    #endregion Public Properties

    #region Public Methods

    public static EarthEllipsoid FromAWithAlpha(double a, double alpha, string name = "UnnamedEllipsoid")
    {
        var b = a * (1 - alpha);
        var e = GetEccentricity(a, b);
        return new EarthEllipsoid
        {
            Name = name,
            EquatorialRadius = a,
            PolarRadius = b,
            FirstEccentricity = e.E1,
            SecondEccentricity = e.E2,
            Oblateness = alpha
        };
    }

    public static EarthEllipsoid FromAWithB(double a, double b, string name = "UnnamedEllipsoid") => new(a, b, name);

    public static EarthEllipsoid FromAWithE1(double a, double e1, string name = "UnnamedEllipsoid")
    {
        var temp = GetBWithE2(a, e1);
        return new EarthEllipsoid
        {
            Name = name,
            EquatorialRadius = a,
            FirstEccentricity = e1,
            PolarRadius = temp.B,
            SecondEccentricity = temp.E2,
            Oblateness = GetOblateness(a, temp.B)
        };
    }

    public double M(double latitude)
        => A * (1 - E1 * E1) / Pow(W(latitude), 3);

    public double M(Angle latitude)
        => M((double)latitude);

    public double N(double latitude)
        => A / W(latitude);

    public double N(Angle latitude)
        => N((double)latitude);

    public override string ToString()
        => Name;

    public double V(double latitude)
    {
        var t = E2 * Cos(latitude);
        return Sqrt(1 + t * t);
    }

    public double V(Angle latitude)
        => V((double)latitude);

    public double W(double latitude)
    {
        var t = E1 * Sin(latitude);
        return Sqrt(1 - t * t);
    }

    public double W(Angle latitude)
        => W((double)latitude);

    #endregion Public Methods

    #region Private Methods

    public static bool operator ==(EarthEllipsoid left, EarthEllipsoid right)
        => left.Equals(right);

    public static bool operator !=(EarthEllipsoid left, EarthEllipsoid right)
        => !left.Equals(right);

    public bool Equals(EarthEllipsoid other)
        => A == other.A && B == other.B;

    public override bool Equals(object? obj)
        => obj is EarthEllipsoid ellipsoid && Equals(ellipsoid);

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    private static (double B, double E2) GetBWithE2(double a, double e1)
    {
        var b = a * a * (1 - e1 * e1);
        var e2 = e1 * a / b;
        return (b, e2);
    }

    private static (double E1, double E2) GetEccentricity(double a, double b)
    {
        var temp = Sqrt(a * a - b * b);
        return (temp / a, temp / b);
    }

    private static double GetOblateness(double a, double b)
        => (a - b) / a;
    #endregion Private Methods
}