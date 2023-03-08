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

    public Angle(double rad) => _rad = rad;

    #endregion Public Constructors

    #region Public Properties

    public double Degrees
    {
        get => RadToDeg * _rad;
        set => _rad = value * DegToRad;
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

    public double Rads
    {
        get => _rad;
        set => _rad = value;
    }

    #endregion Public Properties

    #region Public Methods

    public static double Cos(Angle angle)
        => Math.Cos(angle._rad);

    public static double Cot(Angle angle)
        => Math.Tan(PI / 2 - angle._rad);

    public static double Cot(double rad)
        => Math.Tan(PI / 2 - rad);

    public static double Csc(Angle angle)
        => 1 / Sin(angle);

    public static double Csc(double rad)
        => 1 / Math.Sin(rad);

    public static explicit operator double(Angle angle)
        => angle.Rads;

    public static Angle FromDegrees(double deg)
        => new(deg * DegToRad);

    public static Angle FromDegrees(int deg, byte min, double sec)
        => new() { DegreesMinutesSeconds = (deg, min, sec) };

    public static Angle FromRads(double rad)
        => new(rad);

    public static Angle operator -(Angle left, Angle right)
        => new(left._rad - right._rad);

    public static Angle operator -(Angle angle)
        => new(-angle._rad);

    public static bool operator !=(Angle left, Angle right)
        => !(left == right);

    public static Angle operator *(double num, Angle angle)
        => new(angle._rad * num);

    public static Angle operator *(Angle angle, double num)
        => new(angle._rad * num);

    public static Angle operator /(Angle angle, double num)
        => new(angle._rad / num);

    public static Angle operator +(Angle left, Angle right)
        => new(left._rad + right._rad);

    public static bool operator <(Angle left, Angle right)
        => left._rad < right._rad;

    public static bool operator ==(Angle left, Angle right)
        => left.Equals(right);

    public static bool operator >(Angle left, Angle right)
        => left._rad > right._rad;

    public static double Sec(Angle angle)
        => 1 / Cos(angle);

    public static double Sec(double rad)
        => 1 / Math.Cos(rad);

    public static double Sin(Angle angle)
        => Math.Sin(angle._rad);

    public static double Tan(Angle angle)
        => Math.Tan(angle._rad);

    public Angle Clamp(Angle min, Angle max)
    {
        if (min > max)
            throw new ArgumentException("The specified range is not legal");
        var rad = _rad;
        rad = IEEERemainder(rad -= min._rad, max._rad - min._rad);
        if (rad > 0) rad += min._rad;
        else rad += max._rad;
        return new(rad);
    }

    public int CompareTo(Angle other)
            => _rad.CompareTo(other._rad);

    public bool Equals(Angle other)
        => _rad == other._rad;

    public override bool Equals(object? obj)
        => obj is Angle angle && Equals(angle);

    public override int GetHashCode()
        => _rad.GetHashCode();

    public override string ToString()
        => $"{Degrees}deg";

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        switch (format)
        {
            case "deg":
                return $"{Degrees.ToString(formatProvider)}deg";

            case "rad":
                return $"{Rads.ToString(formatProvider)}";

            case "dms":
                var dms = DegreesMinutesSeconds;
                return $"[{dms.Degrees.ToString(formatProvider)}deg,{dms.Minutes.ToString(formatProvider)}min,{dms.Seconds.ToString(formatProvider)}sec]";

            default:
                return $"{Degrees.ToString(format, formatProvider)}deg";
        }
    }

    #endregion Public Methods

    #region Private Fields

    private double _rad;

    #endregion Private Fields
}