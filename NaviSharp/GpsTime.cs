using System.Numerics;

namespace NaviSharp;

public readonly record struct GpsTime : IComparable<GpsTime>, IAdditionOperators<GpsTime, TimeSpan, GpsTime>, ISubtractionOperators<GpsTime, GpsTime, TimeSpan>, IEquatable<GpsTime>
{
    public ushort Week { get; init; }
    public double Sow { get; init; }
    public double SecondsSinceEpoch => Week * 604800 + Sow;

    public DateTime DateTime
    {
        get => EpochAsDateTime + new TimeSpan((long)(SecondsSinceEpoch * 10000000));
    }
    static GpsTime()
    {
        EpochAsDateTime = new(1980, 1, 6, 0, 0, 0);
        EpochAsGpsTime = new();
    }
    public GpsTime(double second)
    {
        Week = (ushort)(second / 604800);
        Sow = second - Week * 604800;
    }
    public GpsTime(ushort week, double second)
    {
        Week = week;
        Sow = second;
    }

    public GpsTime(TimeSpan timeSpan)
    {
        var second = timeSpan.TotalSeconds;
        Week = (ushort)(second / 604800);
        Sow = second - Week * 604800;
    }

    public GpsTime(DateTime dateTime)
    {
        var second = (dateTime - EpochAsDateTime).TotalSeconds;
        Week = (ushort)(second / 604800);
        Sow = second - Week * 604800;
    }

    public static DateTime EpochAsDateTime { get; }

    public static GpsTime EpochAsGpsTime { get; }

    public static GpsTime Now => new(DateTime.Now);

    public int CompareTo(GpsTime other)
    {
        var flag = Week.CompareTo(other.Week);
        return flag == 0 ? Sow.CompareTo(other.Sow) : flag;
    }

    public static GpsTime operator +(GpsTime left, TimeSpan right)
        => new(left.SecondsSinceEpoch + right.TotalSeconds);

    public static TimeSpan operator -(GpsTime left, GpsTime right)
        => new((long)((left.SecondsSinceEpoch - right.SecondsSinceEpoch) * 10000000));

    public static bool operator >(GpsTime left, GpsTime right)
        => left.CompareTo(right) > 0;

    public static bool operator <(GpsTime left, GpsTime right)
        => left.CompareTo(right) < 0;
    public static bool operator >=(GpsTime left, GpsTime right)
        => left.CompareTo(right) >= 0;
    public static bool operator <=(GpsTime left, GpsTime right)
        => left.CompareTo(right) <= 0;
}