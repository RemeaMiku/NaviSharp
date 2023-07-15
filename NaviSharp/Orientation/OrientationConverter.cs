// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp.Orientation;

public static class OrientationConverter
{
    #region Public Methods

    public static EulerAngles ToEulerAngles(Matrix<double> rotationMatrix)
    {
        Validate(rotationMatrix);
        var c31 = rotationMatrix.At(2, 0);
        var c32 = rotationMatrix.At(2, 1);
        var c33 = rotationMatrix.At(2, 2);
        var pitch = Atan(-c31 / Sqrt(c32 * c32 + c33 * c33));
        if (Abs(pitch) == OneHalfOfPI)
            return new EulerAngles(double.NaN, pitch, double.NaN);
        var c21 = rotationMatrix.At(1, 0);
        var c11 = rotationMatrix.At(0, 0);
        var roll = Atan2(c32, c33);
        var yaw = Atan2(c21, c11);
        if (yaw < 0)
            yaw += DoublePI;
        return new(yaw, pitch, roll);
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Rotation_formalisms_in_three_dimensions
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public static EulerAngles ToEulerAngles(Quaternion<double> q)
    {
        var yaw = Atan2(2 * (q.R * q.K + q.I * q.J), 1 - 2 * (q.J * q.J + q.K * q.K));
        var pitch = Asin(2 * (q.R * q.J - q.K * q.I));
        var roll = Atan2(2 * (q.R * q.I + q.J * q.K), 1 - 2 * (q.I * q.I + q.J * q.J));
        if (yaw < 0)
            yaw += DoublePI;
        return new(yaw, pitch, roll);
    }

    public static RotationMatrix ToRotationMatrix(EulerAngles eulerAngles)
    {
        var yaw = eulerAngles.Yaw;
        var pitch = eulerAngles.Pitch;
        var roll = eulerAngles.Roll;
        var sinPsi = Sin(yaw);
        var cosPsi = Cos(yaw);
        var sinTheta = Sin(pitch);
        var cosTheta = Cos(pitch);
        var sinPhi = Sin(roll);
        var cosPhi = Cos(roll);
        return new(new[,]
        {
            {cosTheta*cosPsi,-cosPhi*sinPsi+sinPhi*sinTheta*cosPsi,sinPhi*sinPsi+cosPhi*sinTheta*cosPsi},
            {cosTheta*sinPsi,cosPhi*cosPsi+sinPhi*sinTheta*sinPsi,-sinPhi*cosPsi+cosPhi*sinTheta*sinPsi },
            {-sinTheta,sinPhi*cosTheta,cosPhi*cosTheta }
        });
    }

    public static RotationMatrix ToRotationMatrix(Quaternion<double> q)
    {
        var r = q.R;
        var i = q.I;
        var j = q.J;
        var k = q.K;
        var q1q1 = r * r;
        var q2q2 = i * i;
        var q3q3 = j * j;
        var q4q4 = k * k;
        var q1q2 = r * i;
        var q1q3 = r * j;
        var q1q4 = r * k;
        var q2q3 = i * j;
        var q2q4 = i * k;
        var q3q4 = j * k;
        return new(new[,]
        {
            {q1q1+q2q2-q3q3-q4q4,2*(q2q3-q1q4), 2*(q2q4+q1q3) },
            { 2*(q2q3+q1q4),q1q1-q2q2+q3q3-q4q4,  2*(q3q4-q1q2)},
            {  2*(q2q4-q1q3), 2*(q3q4+q1q2),q1q1-q2q2-q3q3+q4q4}
        });
    }

    public static RotationMatrix ToRotationMatrix(Vector<double> rotationVector)
    {
        Validate(rotationVector);
        var theta = rotationVector.Norm();
        var axis = rotationVector / theta;
        var sin = Sin(theta);
        var cos = Cos(theta);
        var temp = 1 - cos;
        var e1 = axis.At(0);
        var e2 = axis.At(1);
        var e3 = axis.At(2);
        var c11 = temp * e1 * e1 + cos;
        var c12 = temp * e1 * e2 - e3 * sin;
        var c13 = temp * e1 * e3 + e2 * sin;
        var c21 = temp * e2 * e1 + e3 * sin;
        var c22 = temp * e2 * e2 + cos;
        var c23 = temp * e2 * e3 - e1 * sin;
        var c31 = temp * e3 * e1 - e2 * sin;
        var c32 = temp * e3 * e2 + e1 * sin;
        var c33 = temp * e3 * e3 + cos;
        return new(c11, c12, c13, c21, c22, c23, c31, c32, c33);
    }

    public static Quaternion<double> ToQuaternion(EulerAngles eulerAngles)
    {
        var yaw = eulerAngles.Yaw;
        var pitch = eulerAngles.Pitch;
        var roll = eulerAngles.Roll;
        var sinpsi2 = Sin(yaw / 2);
        var cospsi2 = Cos(yaw / 2);
        var sintheta2 = Sin(pitch / 2);
        var costheta2 = Cos(pitch / 2);
        var sinphi2 = Sin(roll / 2);
        var cosphi2 = Cos(roll / 2);
        var q1 = cosphi2 * costheta2 * cospsi2 + sinphi2 * sintheta2 * sinpsi2;
        var q2 = sinphi2 * costheta2 * cospsi2 - cosphi2 * sintheta2 * sinpsi2;
        var q3 = cosphi2 * sintheta2 * cospsi2 + sinphi2 * costheta2 * sinpsi2;
        var q4 = cosphi2 * costheta2 * sinpsi2 - sinphi2 * sintheta2 * cospsi2;
        return new(q1, q2, q3, q4);
    }

    public static Quaternion<double> ToQuaternion(Matrix<double> rotationMatrix)
    {
        Validate(rotationMatrix);
        var trace = rotationMatrix.Trace();
        var c11 = rotationMatrix.At(0, 0);
        var c22 = rotationMatrix.At(1, 1);
        var c33 = rotationMatrix.At(2, 2);
        var p1 = 1 + trace;
        var temp = 1 - trace;
        var p2 = temp + 2 * c11;
        var p3 = temp + 2 * c22;
        var p4 = temp + 2 * c33;
        var max = Max(p1, Max(p2, Max(p3, p4)));
        var c32 = rotationMatrix.At(2, 1);
        var c23 = rotationMatrix.At(1, 2);
        var c13 = rotationMatrix.At(0, 2);
        var c31 = rotationMatrix.At(2, 0);
        var c21 = rotationMatrix.At(1, 0);
        var c12 = rotationMatrix.At(0, 1);
        Quaternion<double>? q = default;
        if (max == p1)
        {
            var q1 = 0.5 * Sqrt(p1);
            var v = new Vector<double>(c32 - c23, c13 - c31, c21 - c12) / (4 * q1);
            q = new(q1, v);
        }
        if (max == p2)
        {
            var q2 = 0.5 * Sqrt(p2);
            var div = 4 * q2;
            var q3 = (c21 + c12) / div;
            var q4 = (c13 + c31) / div;
            var q1 = (c32 - c23) / div;
            q = new(q1, q2, q3, q4);
        }
        if (max == p3)
        {
            var q3 = 0.5 * Sqrt(p3);
            var div = 4 * q3;
            var q4 = (c32 + c23) / div;
            var q1 = (c13 - c31) / div;
            var q2 = (c12 + c21) / div;
            q = new(q1, q2, q3, q4);
        }
        if (max == p4)
        {
            var q4 = 0.5 * Sqrt(p4);
            var div = 4 * q4;
            var q1 = (c21 - c12) / div;
            var q2 = (c13 + c31) / div;
            var q3 = (c32 + c23) / div;
            q = new(q1, q2, q3, q4);
        }
        if (!q.HasValue)
            throw new Exception("Failed to convert the rotation matrix to a quaternion.");
        var result = q.Value.R < 0 ? -q.Value : q.Value;
        return result;
    }

    public static Quaternion<double> ToQuaternion(Vector<double> rotationVector)
    {
        Validate(rotationVector);
        var half = 0.5 * rotationVector;
        var norm = half.Norm();
        if (norm == 0)
            return new(1, 0, 0, 0);
        return new(Cos(norm), Sin(norm) / norm * half);
    }

    public static RotationVector ToRotationVector(Quaternion<double> q)
    {
        var axis = q.IVector.Unitization();
        var theta = 2 * Acos(q.R);
        return new(axis, new(theta));
    }

    public static RotationVector ToRotationVector(Matrix<double> rotationMatrix)
    {
        var c11 = rotationMatrix.At(0, 0);
        var c12 = rotationMatrix.At(0, 1);
        var c13 = rotationMatrix.At(0, 2);
        var c21 = rotationMatrix.At(1, 0);
        var c22 = rotationMatrix.At(1, 1);
        var c23 = rotationMatrix.At(1, 2);
        var c31 = rotationMatrix.At(2, 0);
        var c32 = rotationMatrix.At(2, 1);
        var c33 = rotationMatrix.At(2, 2);
        var theta = Acos((c11 + c22 + c33 - 1) / 2);
        var doubleSin = 2 * Sin(theta);
        var e1 = (c32 - c23) / doubleSin;
        var e2 = (c13 - c31) / doubleSin;
        var e3 = (c21 - c12) / doubleSin;
        return new(e1, e2, e3, new(theta));
    }

    #endregion Public Methods

    #region Private Methods

    private static void Validate(Matrix<double> matrix)
    {
        if (!matrix.IsSizeOf(3, 3))
            throw new ArgumentException("The matrix must be a square matrix of order 3");
    }
    private static void Validate(Vector<double> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("The vector must be 3 dimensional");
    }

    #endregion Private Methods
}