// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp.Time;

internal static class LeapSecond
{
    #region Public Methods

    public static ushort GetLeapSeconds(UtcTime utcTime)
    {
        if (utcTime >= _leapSecondsData[^1].Utc)
            return _leapSecondsData[^1].LeapSeconds;
        return _leapSecondsData.FindLast(data => utcTime >= data.Utc).LeapSeconds;
    }

    public static ushort GetLeapSeconds(GpsTime gpsTime)
    {
        if (gpsTime >= GpsTime.FromUtc(_leapSecondsData[^1].Utc))
            return _leapSecondsData[^1].LeapSeconds;
        return _leapSecondsData.FindLast(data => gpsTime >= GpsTime.FromUtc(data.Utc)).LeapSeconds;
    }

    #endregion Public Methods

    #region Private Fields

    private static readonly List<(UtcTime Utc, ushort LeapSeconds)> _leapSecondsData = new(27)
    {
        ( new (1972,7,1,0,0,0,TimeSpan.Zero), 1),
        ( new (1973,1,1,0,0,0,TimeSpan.Zero), 2),
        ( new (1974,1,1,0,0,0,TimeSpan.Zero), 3),
        ( new (1975,1,1,0,0,0,TimeSpan.Zero), 4),
        ( new (1976,1,1,0,0,0,TimeSpan.Zero), 5),
        ( new (1977,1,1,0,0,0,TimeSpan.Zero), 6),
        ( new (1978,1,1,0,0,0,TimeSpan.Zero), 7),
        ( new (1979,1,1,0,0,0,TimeSpan.Zero), 8),
        ( new (1980,1,1,0,0,0,TimeSpan.Zero), 9),
        ( new (1981,7,1,0,0,0,TimeSpan.Zero), 10),
        ( new (1982,7,1,0,0,0,TimeSpan.Zero), 11),
        ( new (1983,7,1,0,0,0,TimeSpan.Zero), 12),
        ( new (1985,7,1,0,0,0,TimeSpan.Zero), 13),
        ( new (1988,1,1,0,0,0,TimeSpan.Zero), 14),
        ( new (1990,1,1,0,0,0,TimeSpan.Zero), 15),
        ( new (1991,1,1,0,0,0,TimeSpan.Zero), 16),
        ( new (1992,7,1,0,0,0,TimeSpan.Zero), 17),
        ( new (1993,7,1,0,0,0,TimeSpan.Zero), 18),
        ( new (1994,7,1,0,0,0,TimeSpan.Zero), 19),
        ( new (1996,1,1,0,0,0,TimeSpan.Zero), 20),
        ( new (1997,7,1,0,0,0,TimeSpan.Zero), 21),
        ( new (1999,1,1,0,0,0,TimeSpan.Zero), 22),
        ( new (2006,1,1,0,0,0,TimeSpan.Zero), 23),
        ( new (2009,1,1,0,0,0,TimeSpan.Zero), 24),
        ( new (2012,7,1,0,0,0,TimeSpan.Zero), 25),
        ( new (2015,7,1,0,0,0,TimeSpan.Zero), 26),
        ( new (2017,1,1,0,0,0,TimeSpan.Zero), 27),
    };

    #endregion Private Fields
}
