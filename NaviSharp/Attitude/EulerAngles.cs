// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp;

public readonly partial record struct EulerAngles : IFormattable
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
        if (Yaw <= -StraightAngle || Yaw > StraightAngle)
            throw new ArgumentException($"{nameof(Yaw)} must be in the range of (-180°,180°]");
        if (Pitch < -RightAngle || Pitch > RightAngle)
            throw new ArgumentException($"{nameof(Pitch)} must be in the range of [-90°,90°]");
        if (Roll <= -StraightAngle || Roll > StraightAngle)
            throw new ArgumentException($"{nameof(Roll)} must be in the range of (-180°,180°]");
    }

    public override string ToString()
        => $"[Yaw:{Yaw},Pitch:{Pitch},Roll:{Roll}]";

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"[Yaw:{Yaw.ToString(format, formatProvider)},Pitch:{Pitch.ToString(format, formatProvider)},Roll:{Roll.ToString(format, formatProvider)}]";
}