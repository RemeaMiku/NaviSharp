// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviSharp.Orientation;

#pragma warning disable CA2260

public class RotationMatrix : Matrix<double>, IOrientation
{
    public RotationMatrix() : base(3, 3)
    {
        for (int i = 0; i < 3; i++)
            At(i, i, 1);
    }

    public RotationMatrix(double c11, double c12, double c13, double c21, double c22, double c23, double c31, double c32, double c33) : base(3, 3)
    {
        C11 = c11;
        C12 = c12;
        C13 = c13;
        C21 = c21;
        C22 = c22;
        C23 = c23;
        C31 = c31;
        C32 = c32;
        C33 = c33;
    }
    public RotationMatrix(double[,] nums) : base(nums)
    {
        if (nums.GetLength(0) != 3 || nums.GetLength(1) != 3)
            throw new ArgumentException("The matrix must be a square matrix of order 3.");
    }
    public RotationMatrix(Matrix<double> matrix) : base(3, 3, matrix.Data)
    {
        if (!matrix.IsSizeOf(3, 3))
            throw new ArgumentException("The matrix must be a square matrix of order 3.");
    }
    public double C11
    {
        get => At(0, 0);
        set => At(0, 0, value);
    }
    public double C12
    {
        get => At(0, 1);
        set => At(0, 1, value);
    }
    public double C13
    {
        get => At(0, 2);
        set => At(0, 2, value);
    }
    public double C21
    {
        get => At(1, 0);
        set => At(1, 0, value);
    }
    public double C22
    {
        get => At(1, 1);
        set => At(1, 1, value);
    }
    public double C23
    {
        get => At(1, 2);
        set => At(1, 2, value);
    }
    public double C31
    {
        get => At(2, 0);
        set => At(2, 0, value);
    }
    public double C32
    {
        get => At(2, 1);
        set => At(2, 1, value);
    }
    public double C33
    {
        get => At(2, 2);
        set => At(2, 2, value);
    }

    public EulerAngles ToEulerAngles()
    => OrientationConverter.ToEulerAngles(this);

    public RotationMatrix ToRotationMatrix() => this;

    public RotationVector ToRotationVector()
    => OrientationConverter.ToRotationVector(this);

    public Quaternion<double> ToQuaternion()
    => OrientationConverter.ToQuaternion(this);
}
