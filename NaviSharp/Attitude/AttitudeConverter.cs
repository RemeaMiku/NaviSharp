// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public partial record struct EulerAngle
{
    public Quaternion<T> ToQuaternion<T>() where T : struct, IFloatingPoint<T>
    {
        var sinpsi2 = Sin(Yaw / 2);
        var cospsi2 = Cos(Yaw / 2);
        var sintheta2 = Sin(Pitch / 2);
        var costheta2 = Cos(Pitch / 2);
        var sinphi2 = Sin(Roll / 2);
        var cosphi2 = Cos(Roll / 2);
        return new Quaternion<T>
        (
            (dynamic)cosphi2 * costheta2 * cospsi2 + sinphi2 * sintheta2 * sinpsi2,
            (dynamic)sinphi2 * costheta2 * cospsi2 - cosphi2 * sintheta2 * sinpsi2,
            (dynamic)cosphi2 * sintheta2 * cospsi2 + sinphi2 * costheta2 * sinpsi2,
            (dynamic)cosphi2 * costheta2 * sinpsi2 - sinphi2 * sintheta2 * cospsi2
        );
    }
}

public partial struct Quaternion<T>
{
    #region Public Methods

    public Matrix<T> ToRotationMatrix()
    {
        var q1q1 = R * R;
        var q2q2 = I * I;
        var q3q3 = J * J;
        var q4q4 = K * K;
        var q1q2 = R * I;
        var q1q3 = R * J;
        var q1q4 = R * K;
        var q2q3 = I * J;
        var q2q4 = I * K;
        var q3q4 = J * K;
        return new Matrix<T>(new T[,]
        {
            {q1q1+q2q2-q3q3-q4q4,(dynamic)2*(q2q3-q1q4),(dynamic) 2*(q2q4+q1q3) },
            {(dynamic) 2*(q2q3+q1q4),q1q1-q2q2+q3q3-q4q4, (dynamic) 2*(q3q4-q1q2)},
            { (dynamic) 2*(q2q4-q1q3),(dynamic) 2*(q3q4+q1q2),q1q1-q2q2-q3q3+q4q4}
        });
    }

    public Vector<T> ToVector()
    {
        if (T.IsZero(R))
            return T.Pi * IVector;
        var temp = Sqrt((dynamic)I * I + J * J + K * K) / R;
        var f = Math.Sin(temp) / (2 * temp);
        return IVector / f;
    }

    #endregion Public Methods
}

public partial class Matrix<T>
{
    #region Public Methods

    public EulerAngle ToEulerAngle()
    {
        if (!IsSizeOf(3, 3))
            throw new ArgumentException("The matrix must be a square matrix of order 3");
        var pitch = Atan(-At(2, 0) / Sqrt((dynamic)At(2, 1) * At(2, 1) + At(2, 2) * At(2, 2)));
        if (Abs(pitch) == OneHalfOfPI)
            return new EulerAngle(double.NaN, pitch, double.NaN);
        var roll = Atan2((dynamic)At(2, 1), (dynamic)At(2, 2));
        var yaw = Atan2((dynamic)At(1, 0), (dynamic)At(0, 0));
        return new EulerAngle(yaw, pitch, roll);
    }

    #endregion Public Methods
}

public partial class Vector<T>
{
    #region Public Methods

    public Quaternion<T> ToQuaternion()
    {
        if (!IsSizeOf(3))
            throw new ArgumentException("The vector must be 3 dimensional");
        Vector<T> temp = (dynamic)0.5 * this;
        var norm = temp.Norm();
        if (T.IsZero(norm))
            return new Quaternion<T>(T.One, T.Zero, T.Zero, T.Zero);
        return new Quaternion<T>(Math.Cos((dynamic)norm), Math.Sin((dynamic)norm) * temp.Unitization());
    }

    #endregion Public Methods
}