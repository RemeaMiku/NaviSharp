// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;
using NaviSharp.Orientation;

namespace NaviSharp;

public partial struct Quaternion<T> :
    IOrientation,
    IMultiplyOperators<Quaternion<T>, Quaternion<T>, Quaternion<T>>,
    IAdditionOperators<Quaternion<T>, Quaternion<T>, Quaternion<T>>,
    ISubtractionOperators<Quaternion<T>, Quaternion<T>, Quaternion<T>>,
    IMultiplyOperators<Quaternion<T>, T, Quaternion<T>>,
    IUnaryNegationOperators<Quaternion<T>, Quaternion<T>>
    where T : struct, IFloatingPoint<T>
{
    #region Public Constructors

    public Quaternion()
    {
        R = T.Zero;
        IVector = new Vector<T>(3);
    }

    public Quaternion(T real, T i, T j, T k)
    {
        R = real;
        IVector = new Vector<T>(i, j, k);
    }

    public Quaternion(T real, Vector<T> imaginary)
    {
        if (!imaginary.IsSizeOf(3))
            throw new ArgumentException("The vector must have three dimensions");
        R = real;
        IVector = imaginary;
    }

    #endregion Public Constructors

    #region Public Properties

    public readonly T I
    {
        get => IVector.At(0);
        set => IVector.At(0, value);
    }

    public Vector<T> IVector { get; init; }

    public readonly T J
    {
        get => IVector.At(1);
        set => IVector.At(1, value);
    }

    public readonly T K
    {
        get => IVector.At(2);
        set => IVector.At(2, value);
    }

    public T R { get; set; }

    #endregion Public Properties

    #region Public Indexers

    public T this[int i]
    {
        readonly get
        {
            return i switch
            {
                0 => R,
                1 => I,
                2 => J,
                3 => K,
                _ => throw new ArgumentOutOfRangeException(nameof(i)),
            };
        }
        set
        {
            switch (i)
            {
                case 0:
                    R = value;
                    break;

                case 1:
                    I = value;
                    break;

                case 2:
                    J = value;
                    break;

                case 3:
                    K = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i));
            }
        }
    }

    #endregion Public Indexers

    #region Public Methods

    public static Quaternion<T> operator -(Quaternion<T> left, Quaternion<T> right)
        => new(left.R - right.R, left.IVector - right.IVector);

    public static Quaternion<T> operator -(Quaternion<T> value)
        => new(-value.R, -value.IVector);

    public static Quaternion<T> operator *(Quaternion<T> left, Quaternion<T> right)
        => new(left.R * right.R - left.IVector * right.IVector, left.R * right.IVector + right.R * left.IVector + left.IVector.CrossProduct(right.IVector));

    public static Quaternion<T> operator *(Quaternion<T> left, T right)
            => new(left.R * right, left.IVector * right);
    public static Quaternion<T> operator *(T left, Quaternion<T> right)
        => right * left;

    public static Quaternion<T> operator +(Quaternion<T> left, Quaternion<T> right)
        => new(left.R + right.R, left.IVector + right.IVector);

    public override readonly string ToString()
        => $"[{R} {I} {J} {K}]";

    public readonly EulerAngles ToEulerAngles()
    {
        if (this is Quaternion<double> q)
        {
            return OrientationConverter.ToEulerAngles(q);
        }
        throw new NotImplementedException();
    }

    public readonly RotationMatrix ToRotationMatrix()
    {
        if (this is Quaternion<double> q)
        {
            return OrientationConverter.ToRotationMatrix(q);
        }
        throw new NotImplementedException();
    }

    public readonly RotationVector ToRotationVector()
    {
        if (this is Quaternion<double> q)
        {
            return OrientationConverter.ToRotationVector(q);
        }
        throw new NotImplementedException();
    }

    public readonly Quaternion<double> ToQuaternion()
    {
        if (this is Quaternion<double> q)
        {
            return q;
        }
        throw new NotImplementedException();
    }

    #endregion Public Methods
}