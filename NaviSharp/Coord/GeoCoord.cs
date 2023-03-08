namespace NaviSharp;

public readonly partial record struct GeoCoord : IFormattable
{
    public Angle Latitude { get; init; }
    public Angle Longitude { get; init; }
    public double Height { get; init; }
    public double B => Latitude.Rads;
    public double L => Longitude.Rads;
    public double H => Height;
    public GeoCoord()
    {
        Latitude = ZeroAngle;
        Longitude = ZeroAngle;
        Height = 0;
    }
    public GeoCoord(double latitude, double longitude, double height)
    {
        Latitude = new Angle(latitude).Clamp(-RightAngle, RightAngle);
        Longitude = new Angle(longitude).Clamp(-StraightAngle, StraightAngle);
        Height = height;
    }
    public GeoCoord(Angle latitude, Angle longitude, double height)
    {
        Latitude = latitude.Clamp(-RightAngle, RightAngle); ;
        Longitude = longitude.Clamp(-StraightAngle, StraightAngle);
        Height = height;
    }

    public override string ToString()
        => $"[Lat:{Latitude},Lon:{Longitude},Hgt{Height}]";

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format == "deg" || format == "rad" || format == "dms")
            return $"[Lat:{Latitude.ToString(format, formatProvider)},Lon:{Longitude.ToString(format, formatProvider)},Hgt:{Height.ToString(formatProvider)}]";
        return $"[Lat:{Latitude.ToString(format, formatProvider)},Lon:{Longitude.ToString(format, formatProvider)},Hgt:{Height.ToString(format, formatProvider)}]";
    }

}