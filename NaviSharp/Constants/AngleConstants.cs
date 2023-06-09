﻿namespace NaviSharp;

public partial struct Angle
{
    #region Public Fields

    public const double DegToRad = PI / 180;
    public const double DoublePI = 2 * PI;
    public const double OneHalfOfPI = PI / 2;
    public const double RadToDeg = 180 / PI;

    #endregion Public Fields

    #region Public Properties

    public static Angle RightAngle => new(OneHalfOfPI);
    public static Angle RoundAngle => new(DoublePI);
    public static Angle StraightAngle => new(PI);
    public static Angle ZeroAngle => new(0);

    #endregion Public Properties
}