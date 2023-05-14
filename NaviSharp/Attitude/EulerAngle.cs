namespace NaviSharp;

public readonly partial record struct EulerAngle : IFormattable
{
    public Angle Yaw { get; init; }

    public Angle Pitch { get; init; }

    public Angle Roll { get; init; }

    public EulerAngle()
    {
        Yaw = ZeroAngle;
        Pitch = ZeroAngle;
        Roll = ZeroAngle;
    }

    public EulerAngle(double yaw, double pitch, double roll)
    {
        Yaw = new(yaw);
        Pitch = new(pitch);
        Roll = new(roll);
        Clamp();
    }

    public EulerAngle(Angle yaw, Angle pitch, Angle roll)
    {
        Yaw = yaw;
        Pitch = pitch;
        Roll = roll;
        Clamp();
    }
    private void Clamp()
    {
        Yaw.Clamp(ZeroAngle, RoundAngle);
        Pitch.Clamp(-RightAngle, RightAngle);
        Roll.Clamp(-StraightAngle, StraightAngle);
    }
    public override string ToString()
        => $"[Yaw:{Yaw},Pitch:{Pitch},Roll:{Roll}]";

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"[Yaw:{Yaw.ToString(format, formatProvider)},Pitch:{Pitch.ToString(format, formatProvider)},Roll:{Roll.ToString(format, formatProvider)}]";
}