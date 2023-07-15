// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviSharp.Orientation;

#pragma warning disable CA2260

public class RotationVector : Vector<double>, IOrientation
{
    public RotationVector(Vector<double> axis, Angle angle) : base(3)
    {
        if (!axis.IsSizeOf(3))
            throw new ArgumentException("The vector must be 3 dimensional");
        for (int i = 0; i < 3; i++)
            At(i, axis.At(i) * angle.Radians);
    }
    public RotationVector(double e1, double e2, double e3, Angle angle) : base(angle.Radians * e1, angle.Radians * e2, angle.Radians * e3)
    {

    }
    public RotationVector(Vector<double> rotationVector) : base(rotationVector.Data)
    {
        if (!rotationVector.IsSizeOf(3))
            throw new ArgumentException("The vector must be 3 dimensional");
    }
    public Angle Angle
    {
        get => new(Norm());
        set => DoMult(value.Radians, Axis.Data, Data);
    }
    public Vector<double> Axis
    {
        get => Unitization();
        set => DoMult(Norm(), value.Data, Data);
    }

    public EulerAngles ToEulerAngles()
    => OrientationConverter.ToEulerAngles(ToQuaternion());

    public RotationMatrix ToRotationMatrix()
    => OrientationConverter.ToRotationMatrix(this);

    public RotationVector ToRotationVector() => this;

    public Quaternion<double> ToQuaternion()
    => OrientationConverter.ToQuaternion(this);
}
