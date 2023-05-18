// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp;

public readonly partial record struct GeodeticCoord : IFormattable
{
    public Angle Latitude { get; init; }
    public Angle Longitude { get; init; }
    public double Altitude { get; init; }
    public double B => Latitude.Radians;
    public double L => Longitude.Radians;
    public double H => Altitude;
    public GeodeticCoord()
    {
        Latitude = ZeroAngle;
        Longitude = ZeroAngle;
        Altitude = 0;
    }
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
        => $"[Lat:{Latitude},Lon:{Longitude},Hgt:{Altitude}]";
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
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format == "deg" || format == "rad" || format == "dms")
            return $"[Lat:{Latitude.ToString(format, formatProvider)},Lon:{Longitude.ToString(format, formatProvider)},Hgt:{Altitude.ToString(formatProvider)}]";
        return $"[Lat:{Latitude.ToString(format, formatProvider)},Lon:{Longitude.ToString(format, formatProvider)},Hgt:{Altitude.ToString(format, formatProvider)}]";
    }

}