// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NaviSharp.SpatialReference;
[DebuggerDisplay("Lat = {Latitude.Degrees}°, Lon = {Longitude.Degrees}°, Alt = {Altitude}")]
public readonly partial record struct GeodeticCoord : IFormattable, IParsable<GeodeticCoord>
{
    public Angle Latitude { get; init; }
    public Angle Longitude { get; init; }
    public double Altitude { get; init; }
    public double B => Latitude.Radians;
    public double L => Longitude.Radians;
    public double H => Altitude;
    public GeodeticCoord(double latitude, double longitude, double altitude)
    {
        Latitude = new(latitude);
        Longitude = new(longitude);
        Altitude = altitude;
        ValidateRange();
    }
    public GeodeticCoord(Angle latitude, Angle longitude, double altitude)
    {
        Latitude = latitude; ;
        Longitude = longitude;
        Altitude = altitude;
        ValidateRange();
    }

    public void Deconstruct(out Angle latitude, out Angle longitude, out double altitude)
        => (latitude, longitude, altitude) = (Latitude, Longitude, Altitude);

    private void ValidateRange()
    {
        if (Latitude < -RightAngle || Latitude > RightAngle)
            throw new ArgumentException($"{nameof(Latitude)} must be in the range of [-90°,90°]");
        if (Longitude <= -StraightAngle || Longitude > StraightAngle)
            throw new ArgumentException($"{nameof(Longitude)} must be in the range of (-180°,180°]");
    }
    public override string ToString()
        => $"{Latitude:F8},{Longitude:F8},{Altitude:F3}";
    /// <summary>
    /// Converts this to a formatted string.
    /// </summary>
    /// <param name="format">
    /// <list type="table">
    ///     <listheader>
    ///       <term>Format strings</term>
    ///     </listheader>
    ///     <item>
    ///       <term>"deg"</term>
    ///       <description>Format in degrees follow by a "deg"</description>
    ///     </item>
    ///     <item>
    ///       <term>"rad"</term>
    ///       <description>Format in radians</description>
    ///     </item>
    ///     <item>
    ///       <term>"dms"</term>
    ///       <description>Format in [degrees, minutes, seconds]</description>
    ///     </item>
    ///   </list>
    /// </param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (format == null)
        {
            return ToString();
        }
        if (format == "deg" || format == "dms" || format == "rad")
        {
            return $"{Latitude.ToString(format, formatProvider)},{Longitude.ToString(format, formatProvider)},{Altitude.ToString(formatProvider)}";
        }
        return $"{Latitude.ToString(format, formatProvider)},{Longitude.ToString(format, formatProvider)},{Altitude.ToString(format, formatProvider)}";
    }

    public static GeodeticCoord Parse(string s, IFormatProvider? provider = null)
    {
        var values = s.Split(',', StringSplitOptions.TrimEntries);
        var lat = Angle.Parse(values[0], provider);
        var lon = Angle.Parse(values[1], provider);
        var alt = double.Parse(values[2]);
        return new(lat, lon, alt);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out GeodeticCoord result)
    {
        if (string.IsNullOrEmpty(s))
        {
            result = default;
            return false;
        }
        var values = s.Split(',', StringSplitOptions.TrimEntries);
        if (!Angle.TryParse(values[0], provider, out var lat) || !Angle.TryParse(values[1], provider, out var lon) || !double.TryParse(values[2], provider, out var alt))
        {
            result = default;
            return false;
        }
        result = new(lat, lon, alt);
        return true;
    }

    public EcefCoord ToEcefCoord(EarthEllipsoid earthEllipsoid)
    => CoordConverter.ToEcef(this, earthEllipsoid);
}