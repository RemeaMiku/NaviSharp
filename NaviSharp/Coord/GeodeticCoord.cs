﻿// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp;

public readonly partial record struct GeodeticCoord : IFormattable
{
    public Angle Latitude { get; init; }
    public Angle Longitude { get; init; }
    public double Height { get; init; }
    public double B => Latitude.Radians;
    public double L => Longitude.Radians;
    public double H => Height;
    public GeodeticCoord()
    {
        Latitude = ZeroAngle;
        Longitude = ZeroAngle;
        Height = 0;
    }
    public GeodeticCoord(double latitude, double longitude, double height)
    {
        Latitude = new(latitude);
        Longitude = new(longitude);
        Height = height;
        Clamp();
    }
    public GeodeticCoord(Angle latitude, Angle longitude, double height)
    {
        Latitude = latitude; ;
        Longitude = longitude;
        Height = height;
        Clamp();
    }

    private void Clamp()
    {
        Latitude.Clamp(-RightAngle, RightAngle);
        Longitude.Clamp(-StraightAngle, StraightAngle);
    }
    public override string ToString()
        => $"[Lat:{Latitude},Lon:{Longitude},Hgt{Height}]";
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
            return $"[Lat:{Latitude.ToString(format, formatProvider)},Lon:{Longitude.ToString(format, formatProvider)},Hgt:{Height.ToString(formatProvider)}]";
        return $"[Lat:{Latitude.ToString(format, formatProvider)},Lon:{Longitude.ToString(format, formatProvider)},Hgt:{Height.ToString(format, formatProvider)}]";
    }

}