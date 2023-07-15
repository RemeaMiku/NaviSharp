// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace NaviSharp;
[DebuggerDisplay("{Radians}rad | {Degrees}° | {DegreesMinutesSeconds.Degrees}°{DegreesMinutesSeconds.Minutes}′{DegreesMinutesSeconds.Seconds}″")]
public partial struct Angle :
    IEquatable<Angle>,
    IComparable<Angle>,
    IAdditionOperators<Angle, Angle, Angle>,
    ISubtractionOperators<Angle, Angle, Angle>,
    IMultiplyOperators<Angle, double, Angle>,
    IDivisionOperators<Angle, double, Angle>,
    IFormattable,
    IParsable<Angle>
{
    #region Public Fields

    public const double RadiansPerDegree = PI / 180;
    public const double DoublePI = 2 * PI;
    public const double OneHalfOfPI = PI / 2;
    public const double DegreesPerRadian = 180 / PI;
    public readonly static Angle RightAngle = new(OneHalfOfPI);
    public readonly static Angle RoundAngle = new(DoublePI);
    public readonly static Angle StraightAngle = new(PI);
    public readonly static Angle ZeroAngle = new(0);

    #endregion Public Fields

    #region Public Constructors

    public Angle(double radians) => Radians = radians;

    public Angle(int degrees, byte minutes, double seconds)
        => DegreesMinutesSeconds = (degrees, minutes, seconds);

    #endregion Public Constructors

    #region Public Properties

    public double Degrees
    {
        readonly get => DegreesPerRadian * Radians;
        set => Radians = value * RadiansPerDegree;
    }

    public (int Degrees, byte Minutes, double Seconds) DegreesMinutesSeconds
    {
        readonly get
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

    public double Radians { get; set; }
    #endregion Public Properties

    #region Public Methods

    public enum AngleRange
    {
        ZeroToRound,
        NegativeStraightToStraight
    }

    public static Angle Map(Angle angle, AngleRange range = AngleRange.ZeroToRound)
    {
        angle.Radians = IEEERemainder(angle.Radians, DoublePI);
        if (angle < ZeroAngle)
            angle += RoundAngle;
        if (range == AngleRange.NegativeStraightToStraight && angle > StraightAngle)
            angle -= RoundAngle;
        return angle;
    }

    public static Angle AddDegrees(Angle angle, double degrees)
    => new(angle.Radians + degrees * RadiansPerDegree);

    public static Angle AddRads(Angle angle, double radians)
    => new(angle.Radians + radians);

    public static double Cos(Angle angle)
        => System.Math.Cos(angle.Radians);

    public static double Cot(Angle angle)
        => System.Math.Tan(PI / 2 - angle.Radians);

    public static double Cot(double radians)
        => System.Math.Tan(PI / 2 - radians);

    public static double Csc(Angle angle)
        => 1 / Sin(angle);

    public static double Csc(double radians)
        => 1 / System.Math.Sin(radians);

    public static explicit operator double(Angle angle)
        => angle.Radians;

    public static Angle FromDegrees(double deg)
        => new(deg * RadiansPerDegree);

    public static Angle FromDegrees(int degrees, byte minutes, double seconds)
        => new() { DegreesMinutesSeconds = (degrees, minutes, seconds) };

    public static Angle FromRads(double radians)
        => new(radians);

    public static Angle operator -(Angle left, Angle right)
        => new(left.Radians - right.Radians);

    public static Angle operator -(Angle angle)
        => new(-angle.Radians);

    public static bool operator !=(Angle left, Angle right)
        => !(left == right);

    public static Angle operator *(double num, Angle angle)
        => new(angle.Radians * num);

    public static Angle operator *(Angle angle, double num)
        => new(angle.Radians * num);

    public static Angle operator /(Angle angle, double num)
        => new(angle.Radians / num);

    public static Angle operator +(Angle left, Angle right)
        => new(left.Radians + right.Radians);

    public static bool operator <(Angle left, Angle right)
        => left.Radians < right.Radians;

    public static bool operator ==(Angle left, Angle right)
        => left.Equals(right);

    public static bool operator >(Angle left, Angle right)
        => left.Radians > right.Radians;

    public static bool operator >=(Angle left, Angle right)
        => left.Radians >= right.Radians;

    public static bool operator <=(Angle left, Angle right)
        => left.Radians <= right.Radians;

    public static double Sec(Angle angle)
        => 1 / Cos(angle);

    public static double Sec(double radians)
        => 1 / System.Math.Cos(radians);

    public static double Sin(Angle angle)
        => System.Math.Sin(angle.Radians);

    public static double Tan(Angle angle)
        => System.Math.Tan(angle.Radians);

    public static Angle Clamp(Angle angle, Angle min, Angle max)
    {
        if (min > max)
            throw new ArgumentOutOfRangeException($"{nameof(min)}({min.Degrees}deg) can't be greater than {nameof(max)}({max.Degrees}deg).");
        return new(System.Math.Clamp(angle.Radians, min.Radians, max.Radians));
    }

    public static Angle Parse(string s, IFormatProvider? provider = null)
    => FromDegrees(double.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Angle result)
    {
        if (!double.TryParse(s, provider, out var deg))
        {
            result = default;
            return false;
        }
        result = FromDegrees(deg);
        return true;
    }

    public readonly void Deconstruct(out int degrees, out byte minutes, out double seconds)
    {
        var totalSeconds = 3600M * (decimal)Degrees;
        (degrees, minutes, seconds) = ((int)Abs(totalSeconds % 60M), (byte)Abs(totalSeconds / 60M % 60M), (double)(totalSeconds / 3600M));
    }
    public void AddDegrees(double degrees) => Radians += degrees * RadiansPerDegree;

    public void AddRadians(double radians) => Radians += radians;

    public readonly int CompareTo(Angle other)
            => Radians.CompareTo(other.Radians);

    public readonly bool Equals(Angle other)
        => Radians == other.Radians;

    public override readonly bool Equals(object? obj)
        => obj is Angle angle && Equals(angle);

    public override readonly int GetHashCode()
        => Radians.GetHashCode();

    public override readonly string ToString()
        => $"{Degrees:F8}";
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
    public readonly string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (format == null)
            return ToString();
        switch (format?.ToLower())
        {
            case "deg":
                return $"{Degrees.ToString(formatProvider)}";
            case "rad":
                return $"{Radians.ToString(formatProvider)}";
            case "dms":
                var dms = DegreesMinutesSeconds;
                return $"{dms.Degrees.ToString(formatProvider)},{dms.Minutes.ToString(formatProvider)},{dms.Seconds.ToString(formatProvider)}";
            default:
                return $"{Degrees.ToString(format, formatProvider)}";
        }
    }
    #endregion Public Methods

}