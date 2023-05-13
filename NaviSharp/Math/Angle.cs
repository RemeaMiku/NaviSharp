using System.Numerics;

namespace NaviSharp;

public partial struct Angle :
    IEquatable<Angle>,
    IComparable<Angle>,
    IAdditionOperators<Angle, Angle, Angle>,
    ISubtractionOperators<Angle, Angle, Angle>,
    IMultiplyOperators<Angle, double, Angle>,
    IDivisionOperators<Angle, double, Angle>,
    IFormattable
{
    #region Public Constructors

    public Angle(double radians) => _radians = radians;

    public Angle(int degrees, byte minutes, double seconds)
        => DegreesMinutesSeconds = (degrees, minutes, seconds);

    #endregion Public Constructors

    #region Public Properties

    public double Degrees
    {
        get => DegreesPerRadian * _radians;
        set => _radians = value * RadiansPerDegree;
    }

    public (int Degrees, byte Minutes, double Seconds) DegreesMinutesSeconds
    {
        get
        {
            var totalSeconds = 3600M * (decimal)Degrees;
            var seconds = Abs(totalSeconds % 60M);
            var minutes = Abs(totalSeconds / 60M % 60M);
            var degrees = totalSeconds / 3600M;
            return ((int)degrees, (byte)minutes, (double)seconds);
        }
        set
        {
            var sign = value.Degrees < 0 ? -1 : 1;
            Degrees = value.Degrees + sign * (value.Minutes / 60.0 + value.Seconds / 3600);
        }
    }

    public double Radians
    {
        get => _radians;
        set => _radians = value;
    }

    #endregion Public Properties

    #region Public Methods

    public static Angle AddDegrees(Angle angle, double degrees)
    => new(angle._radians + degrees * RadiansPerDegree);

    public static Angle AddRads(Angle angle, double radians)
    => new(angle._radians + radians);

    public void AddDegrees(double degrees)
    => _radians += degrees * RadiansPerDegree;

    public void AddRadians(double radians)
    => _radians += radians;

    public static double Cos(Angle angle)
        => Math.Cos(angle._radians);

    public static double Cot(Angle angle)
        => Math.Tan(PI / 2 - angle._radians);

    public static double Cot(double radians)
        => Math.Tan(PI / 2 - radians);

    public static double Csc(Angle angle)
        => 1 / Sin(angle);

    public static double Csc(double radians)
        => 1 / Math.Sin(radians);

    public static explicit operator double(Angle angle)
        => angle.Radians;

    public static Angle FromDegrees(double deg)
        => new(deg * RadiansPerDegree);

    public static Angle FromDegrees(int degrees, byte minutes, double seconds)
        => new() { DegreesMinutesSeconds = (degrees, minutes, seconds) };

    public static Angle FromRads(double radians)
        => new(radians);

    public static Angle operator -(Angle left, Angle right)
        => new(left._radians - right._radians);

    public static Angle operator -(Angle angle)
        => new(-angle._radians);

    public static bool operator !=(Angle left, Angle right)
        => !(left == right);

    public static Angle operator *(double num, Angle angle)
        => new(angle._radians * num);

    public static Angle operator *(Angle angle, double num)
        => new(angle._radians * num);

    public static Angle operator /(Angle angle, double num)
        => new(angle._radians / num);

    public static Angle operator +(Angle left, Angle right)
        => new(left._radians + right._radians);

    public static bool operator <(Angle left, Angle right)
        => left._radians < right._radians;

    public static bool operator ==(Angle left, Angle right)
        => left.Equals(right);

    public static bool operator >(Angle left, Angle right)
        => left._radians > right._radians;

    public static double Sec(Angle angle)
        => 1 / Cos(angle);

    public static double Sec(double radians)
        => 1 / Math.Cos(radians);

    public static double Sin(Angle angle)
        => Math.Sin(angle._radians);

    public static double Tan(Angle angle)
        => Math.Tan(angle._radians);

    public Angle Clamp(Angle min, Angle max)
    {
        if (min > max)
            throw new ArgumentException("The specified range is not legal");
        var radians = _radians;
        radians = IEEERemainder(radians -= min._radians, max._radians - min._radians);
        if (radians > 0) radians += min._radians;
        else radians += max._radians;
        return new(radians);
    }

    public int CompareTo(Angle other)
            => _radians.CompareTo(other._radians);

    public bool Equals(Angle other)
        => _radians == other._radians;

    public override bool Equals(object? obj)
        => obj is Angle angle && Equals(angle);

    public override int GetHashCode()
        => _radians.GetHashCode();

    public override string ToString()
        => $"{Degrees}deg";
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
    /// <returns></returns>
    public string ToString(string format)
        => ToString(format, null);
    /// <summary>
    /// Converts this to a formatted string.
    /// </summary>
    /// <param name="format">
    ///   <list type="table">
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
        switch (format?.ToLower())
        {
            case "deg":
                return $"{Degrees.ToString(formatProvider)}deg";

            case "rad":
                return $"{Radians.ToString(formatProvider)}";

            case "dms":
                var dms = DegreesMinutesSeconds;
                return $"[{dms.Degrees.ToString(formatProvider)}deg,{dms.Minutes.ToString(formatProvider)}min,{dms.Seconds.ToString(formatProvider)}sec]";

            default:
                return $"{Degrees.ToString(format, formatProvider)}deg";
        }
    }

    #endregion Public Methods

    #region Private Fields

    private double _radians;

    #endregion Private Fields
}