// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public readonly partial record struct GpsTime : IComparable<GpsTime>, IAdditionOperators<GpsTime, TimeSpan, GpsTime>, ISubtractionOperators<GpsTime, GpsTime, TimeSpan>, IEquatable<GpsTime>
{
    public ushort Week { get; init; }
    public double Sow { get; init; }
    public double SecondsSinceEpoch => Week * SecondsPerWeek + Sow;
    public UtcTime Utc => ToUtc(this);
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

    public static UtcTime StartPointAsUtcTime { get; } = new(1980, 1, 6, 0, 0, 0, TimeSpan.Zero);

    public static GpsTime Now => FromUtc(UtcTime.Now);

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

    private const ushort _startPointLeapSeconds = 9;
    public static GpsTime FromUtc(UtcTime utcTime)
    {
        var totalSeconds = (utcTime - StartPointAsUtcTime).TotalSeconds
            + LeapSecond.GetLeapSeconds(utcTime)
            - _startPointLeapSeconds;
        var week = (ushort)(totalSeconds / SecondsPerWeek);
        var sow = totalSeconds - week * SecondsPerWeek;
        return new(week, sow);
    }

    public static UtcTime ToUtc(GpsTime gpsTime)
    {
        var totalSeconds = gpsTime.SecondsSinceEpoch - LeapSecond.GetLeapSeconds(gpsTime) + _startPointLeapSeconds;
        return StartPointAsUtcTime.AddSeconds(totalSeconds).ToOffset(TimeZoneInfo.Local.BaseUtcOffset);
    }
}