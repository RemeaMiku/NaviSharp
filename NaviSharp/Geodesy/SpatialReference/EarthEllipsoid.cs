// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Diagnostics;

namespace NaviSharp.SpatialReference;
[DebuggerDisplay("Name = {Name}, a = {A}, b = {B}, e1 = {E1}, e2 = {E2}, alpha = {Alpha}")]
public partial record class EarthEllipsoid
{
    #region Public Constructors

    public EarthEllipsoid(double a, double b, string? name)
    {
        Name = name;
        A = a;
        B = b;
    }

    #endregion Public Constructors

    #region Public Properties


    public double A { get; init; }
    public double Alpha => GetOblateness(A, B);
    public double B { get; init; }
    public (double E1, double E2) E => GetEccentricity(A, B);
    public double E1 => E.E1;
    public double E2 => E.E2;

    public string? Name { get; init; }

    #endregion Public Properties

    #region Public Methods

    public static EarthEllipsoid FromAWithAlpha(double a, double alpha, string name)
    {
        var b = a * (1 - alpha);
        return new EarthEllipsoid(a, b, name);
    }

    public static EarthEllipsoid FromAWithB(double a, double b, string name)
        => new(a, b, name);

    public static EarthEllipsoid FromAWithE1(double a, double e1, string name)
    {
        var b = GetB(a, e1);
        return new EarthEllipsoid(a, b, name);
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
        => Name ?? "Unnamed";

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

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    private static double GetB(double a, double e1)
    {
        var b = a * a * (1 - e1 * e1);
        return b;
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