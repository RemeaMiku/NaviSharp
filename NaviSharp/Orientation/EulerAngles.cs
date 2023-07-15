// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NaviSharp.Orientation;

namespace NaviSharp;

[DebuggerDisplay("Yaw = {Yaw.Degrees}°, Pitch = {Pitch.Degrees}°, Roll = {Roll.Degrees}°")]
public readonly partial record struct EulerAngles : IOrientation, IFormattable, IParsable<EulerAngles>
{
    public Angle Yaw { get; init; }

    public Angle Pitch { get; init; }

    public Angle Roll { get; init; }

    public EulerAngles()
    {
        Yaw = ZeroAngle;
        Pitch = ZeroAngle;
        Roll = ZeroAngle;
    }

    public EulerAngles(double yaw, double pitch, double roll)
    {
        Yaw = new(yaw);
        Pitch = new(pitch);
        Roll = new(roll);
        ValidateRange();
    }

    public EulerAngles(Angle yaw, Angle pitch, Angle roll)
    {
        Yaw = yaw;
        Pitch = pitch;
        Roll = roll;
        ValidateRange();
    }

    private void ValidateRange()
    {
        if (Yaw < ZeroAngle || Yaw >= RoundAngle)
            throw new ArgumentException($"{nameof(Yaw)} must be in the range of [-0°,360°)");
        if (Pitch < -RightAngle || Pitch > RightAngle)
            throw new ArgumentException($"{nameof(Pitch)} must be in the range of [-90°,90°]");
        if (Roll <= -StraightAngle || Roll > StraightAngle)
            throw new ArgumentException($"{nameof(Roll)} must be in the range of (-180°,180°]");
    }

    public override string ToString()
        => $"{Yaw:F3},{Pitch:F3},{Roll:F3}";

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (format == null)
            return ToString();
        return $"{Yaw.ToString(format, formatProvider)},{Pitch.ToString(format, formatProvider)},{Roll.ToString(format, formatProvider)}";
    }


    public static EulerAngles Parse(string s, IFormatProvider? provider = null)
    {
        var values = s.Split(',', StringSplitOptions.TrimEntries);
        var yaw = Angle.Parse(values[0], provider);
        var pitch = Angle.Parse(values[1], provider);
        var roll = Angle.Parse(values[2], provider);
        return new(yaw, pitch, roll);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out EulerAngles result)
    {
        if (string.IsNullOrEmpty(s))
        {
            result = default;
            return false;
        }
        var values = s.Split(',', StringSplitOptions.TrimEntries);
        if (!Angle.TryParse(values[0], provider, out var yaw) || !Angle.TryParse(values[1], provider, out var pitch) || !Angle.TryParse(values[2], provider, out var roll))
        {
            result = default;
            return false;
        }
        try
        {
            result = new(yaw, pitch, roll);
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
        return true;
    }

    public EulerAngles ToEulerAngles() => this;

    public RotationMatrix ToRotationMatrix()
        => OrientationConverter.ToRotationMatrix(this);

    public RotationVector ToRotationVector()
        => OrientationConverter.ToRotationVector(ToQuaternion());

    public Quaternion<double> ToQuaternion()
        => OrientationConverter.ToQuaternion(this);
}