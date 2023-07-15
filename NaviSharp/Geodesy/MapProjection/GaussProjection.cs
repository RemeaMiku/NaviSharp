// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using NaviSharp.SpatialReference;
namespace NaviSharp.Geodesy.MapProjection;

public class GaussProjection : IMapProjection
{
    #region Public Constructors

    public GaussProjection(Angle longitudeOfCentralMeridian, EarthEllipsoid ellipsoid)
    {
        LongitudeOfCentralMeridian = longitudeOfCentralMeridian;
        _ellipsoid = ellipsoid;
    }

    #endregion Public Constructors

    #region Public Properties

    public EarthEllipsoid Ellipsoid
    {
        get => _ellipsoid;
        set
        {
            if (value != _ellipsoid)
                SetCoefficients();
            _ellipsoid = value;
        }
    }
    public Angle LongitudeOfCentralMeridian
    {
        get => _longitudeOfCentralMeridian;
        set => _longitudeOfCentralMeridian = Clamp(value, -StraightAngle, StraightAngle);
    }

    #endregion Public Properties

    #region Public Methods

    public (double X, double Y) ToPlane(Angle latitude, Angle longitude)
    {
        var B = latitude.Radians;
        var sinB = Sin(B);
        var cosB = Cos(B);
        var N = _ellipsoid.N(B);
        var l = (longitude - LongitudeOfCentralMeridian).Radians;
        var e2_1 = _ellipsoid.E1 * _ellipsoid.E1;
        var e2_2 = _ellipsoid.E2 * _ellipsoid.E2;
        var t = Tan(B);
        var eta2 = e2_2 * Cos(B) * Cos(B);
        var X_0 = _ellipsoid.A * (1 - e2_1) * (_a * B - _b / 2 * Sin(2 * B) + _c / 4 * Sin(4 * B) - _d / 6 * Sin(6 * B) + _e / 8 * Sin(8 * B));
        var X = X_0 + N / 2 * sinB * cosB * l * l + N / 24 * sinB * Pow(cosB, 3) * (5 - t * t + 9 * eta2 + 4 * eta2 * eta2) * Pow(l, 4) + N / 720 * sinB * Pow(cosB, 5) * (61 - 58 * t * t + Pow(t, 4)) * Pow(l, 6);
        var Y = N * cosB * l + N / 6 * Pow(cosB, 3) * (1 - t * t + eta2) * Pow(l, 3) + N / 120 * Pow(cosB, 5) * (5 - 18 * t * t + Pow(t, 4) + 14 * eta2 - 58 * eta2 * t * t) * Pow(l, 5);
        return (X, Y);
    }

    public (Angle Latitude, Angle Longitude) ToEllipsoid(double x, double y)
    {
        var e2_1 = _ellipsoid.E1 * _ellipsoid.E1;
        var B_f0 = x / (_ellipsoid.A * _a * (1 - e2_1));
        var GetB_f1 = (double B_f0) => x / (_ellipsoid.A * _a * (1 - e2_1)) + 1 / _a * (_b / 2 * Sin(2 * B_f0)) - _c / 4 * Sin(4 * B_f0) + _d / 6 * Sin(6 * B_f0) - _e / 8 * Sin(8 * B_f0);
        var B_f1 = GetB_f1(B_f0);
        while (Abs(B_f1 - B_f0) >= FromDegrees(0, 0, 0.0001).Radians)
        {
            B_f0 = B_f1;
            B_f1 = GetB_f1(B_f0);
        }
        var M = _ellipsoid.M(B_f1);
        var N = _ellipsoid.N(B_f1);
        var t = Tan(B_f1);
        var eta2 = _ellipsoid.E2 * _ellipsoid.E2 * Cos(B_f1) * Cos(B_f1);
        var B = B_f1 - y * y / (2 * M * N) * t * (1 - y * y / (12 * N * N) * (5 + eta2 + 3 * t * t - 9 * eta2 * t * t) + Pow(y, 4) / (360 * Pow(N, 4)) * (61 + 90 * t * t + 45 * Pow(t, 4)));
        var L = LongitudeOfCentralMeridian.Radians + y / (N * Cos(B_f1)) * (1 - y * y / (6 * N * N) * (1 + eta2 + 2 * t * t) + Pow(y, 4) / (120 * Pow(N, 4)) * (5 + 6 * eta2 + 28 * t * t + 8 * eta2 * t * t + 24 * Pow(t, 4)));
        return (new(B), new(L));
    }

    #endregion Public Methods

    #region Private Fields

    private double _a;
    private double _b;
    private double _c;
    private double _d;
    private double _e;
    private EarthEllipsoid _ellipsoid;
    private Angle _longitudeOfCentralMeridian;

    #endregion Private Fields

    #region Private Methods

    private void SetCoefficients()
    {
        var e2 = _ellipsoid.E1 * _ellipsoid.E1;
        var e4 = e2 * e2;
        var e6 = e4 * e2;
        var e8 = e6 * e2;
        var e10 = e8 * e2;
        _a = 1 + 3 / 4.0 * e2 + 45 / 64.0 * e4 + 175 / 256.0 * e6 + 11025 / 16384.0 * e8 + 43659 / 65536.0 * e10;
        _b = 3 / 4.0 * e2 + 15 / 16.0 * e4 + 525 / 512.0 * e6 + 2205 / 2048.0 * e8 + 72765 / 65536.0 * e10;
        _c = 15 / 64.0 * e4 + 105 / 256.0 * e6 + 2205 / 4096.0 * e8 + 10395 / 16384.0 * e10;
        _d = 35 / 512.0 * e6 + 315 / 2048.0 * e8 + 31185 / 131072.0 * e10;
        _e = 315 / 16384.0 * e8 + 3645 / 65536.0 * e10;
    }

    #endregion Private Methods

}
