using System.Numerics;

namespace NaviSharp;

public readonly record struct GpsTime : IComparable<GpsTime>, IAdditionOperators<GpsTime, TimeSpan, GpsTime>, ISubtractionOperators<GpsTime, GpsTime, TimeSpan>, IEquatable<GpsTime>
{
    public ushort Week { get; init; }
    public double Sow { get; init; }
    public TimeSpan TimeSpanSinceEpoch => TimeSpan.FromSeconds(SecondsSinceEpoch);

    public double SecondsSinceEpoch => Week * SecondsPerWeek + Sow;
    public UtcTime Utc => StartPointAsUtcTime.AddSeconds(SecondsSinceEpoch);
    public GpsTime(double secondsSinceEpoch)
    {
        Week = (ushort)(secondsSinceEpoch / SecondsPerWeek);
        Sow = secondsSinceEpoch - Week * SecondsPerWeek;
    }
    public GpsTime(ushort week, double second)
    {
        if (second >= SecondsPerWeek)
            throw new ArgumentException("SOW must be less than 604,800.", nameof(second));
        Week = week;
        Sow = second;
    }

    public GpsTime(TimeSpan timeSpan)
    {
        var second = timeSpan.TotalSeconds;
        Week = (ushort)(second / SecondsPerWeek);
        Sow = second - Week * SecondsPerWeek;
    }

    public GpsTime(UtcTime utcTime)
    {
        var second = (utcTime - StartPointAsUtcTime).TotalSeconds;
        Week = (ushort)(second / SecondsPerWeek);
        Sow = second - Week * SecondsPerWeek;
    }

    public static UtcTime StartPointAsUtcTime { get; } = new(1980, 1, 6, 0, 0, 0, new());

    public static GpsTime Now => new(UtcTime.UtcNow);

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