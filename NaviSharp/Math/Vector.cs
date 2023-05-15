// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public partial class Vector<T> :
    MatrixVectorBase<Vector<T>, T>,
    IMultiplyOperators<Vector<T>,
    Vector<T>, T>
    where T : struct, IFloatingPoint<T>
{

    #region Public Constructors

    static Vector()
    {
        Empty = new();
    }

    public Vector() : base()
    {
        IsColumn = true;
    }

    public Vector(int dimension, bool isColumn = true) : base(dimension)
    {
        IsColumn = isColumn;
    }

    public Vector(int dimension, T num, bool isColumn = true) : base(dimension, num)
    {
        IsColumn = isColumn;
    }

    public Vector(T[] nums, MatrixVectorConstructMode mode = MatrixVectorConstructMode.Ref) : base(nums, mode)
    {
        IsColumn = true;
    }

    #endregion Public Constructors

    #region Public Properties

    public static Vector<T> Empty { get; }
    public int Dimension => Count;

    public bool IsColumn { get; set; }

    #endregion Public Properties

    #region Public Indexers

    public T this[int i]
    {
        get
        {
            ValidateRange(i);
            return _data[i];
        }
        set
        {
            ValidateRange(i);
            _data[i] = value;
        }
    }

    #endregion Public Indexers

    #region Public Methods

    public static implicit operator Matrix<T>(Vector<T> vector)
    {
        if (vector.IsColumn)
            return new(vector.Dimension, 1, vector._data);
        else
            return new(1, vector.Dimension, vector._data);
    }

    public static T operator *(Vector<T> left, Vector<T> right)
    {
        if (!left.IsSizeOf(right))
            throw new ArgumentException("The dimensions of left and right vectors are not equal");
        return DoDot(left._data, right._data);
    }

    public static Vector<T> operator *(Matrix<T> left, Vector<T> right)
    {
        if (left.ColumnCount != right.Dimension)
            throw new ArgumentException("");
        var result = new T[left.RowCount];
        for (int i = 0; i < left.RowCount; i++)
        {
            result[i] = DoDot(left[i], right._data);
        }
        return new(result);
    }

    public static Vector<T> operator *(T left, Vector<T> right) => right * left;

    public static Vector<T> Random(int dimension)
    {
        var result = new Vector<T>(dimension);
        result.AssignRandom();
        return result;
    }

    public static Vector<T> Unit(int order)
    {
        return new Vector<T>(order, T.One);
    }

    public T At(int i)
    {
        return _data[i];
    }

    public void At(int i, T num)
    {
        _data[i] = num;
    }

    public Vector<T> Combine(Vector<T> other) => Vector<T>.FromVectorArray(this, other);

    public T[] GetRange(int startIndex, int count)
    {
        ValidateRange(startIndex);
        ValidateRange(startIndex + count - 1);
        var array = new T[count];
        Array.Copy(_data, startIndex, array, 0, count);
        return array;
    }

    public T[] GetRange(int count) => GetRange(0, count);

    public bool IsSizeOf(int dimension)
    {
        return Dimension == dimension;
    }

    public bool IsSizeOf(Vector<T> other)
    {
        return Dimension == other.Dimension;
    }

    public T Norm()
    {
        var vectorSize = System.Numerics.Vector<T>.Count;
        int i;
        T result = T.Zero;
        for (i = 0; i <= Dimension - vectorSize; i += vectorSize)
        {
            var vector = new System.Numerics.Vector<T>(_data, i);
            result += Vector.Sum(vector * vector);
        }
        for (; i < Dimension; i++)
        {
            result += _data[i] * _data[i];
        }
        return Sqrt((dynamic)result);
    }

    public Vector<T> OuterProduct(Vector<T> other)
    {
        if (!IsSizeOf(3) || !other.IsSizeOf(3))
            throw new ArgumentException("Only 3-dimensional vectors are supported");
        return new Vector<T>(new T[] { At(1) * other.At(2) - At(2) * other.At(1), At(2) * other.At(0) - At(0) * other.At(2), At(0) * other.At(1) - At(1) * other.At(0) });
    }

    public void SetRange(T[] sourceArray, int sourceIndex, int vectorIndex, int count)
    {
        ValidateRange(vectorIndex);
        ValidateRange(vectorIndex + count - 1);
        Array.Copy(sourceArray, sourceIndex, _data, vectorIndex, count);
    }

    public Vector<T> SubVector(int startIndex, int count)
    {
        ValidateRange(startIndex);
        ValidateRange(startIndex + count - 1);
        var array = new T[count];
        Array.Copy(_data, startIndex, array, 0, count);
        return new(array);
    }

    public Vector<T> SubVector(int count) => SubVector(0, count);

    public override string ToString()
    {
        if (IsColumn)
            return Environment.NewLine + (string.Join(Environment.NewLine, _data));
        else
            return Environment.NewLine + string.Join(' ', _data);
    }

    public Vector<T> Transpose()
    {
        return new Vector<T>(_data, MatrixVectorConstructMode.Copy)
        {
            IsColumn = !IsColumn
        };
    }

    public Vector<T> Unitization()
    {
        return this / Norm();
    }

    #endregion Public Methods

    #region Private Methods

    private void ValidateRange(int i)
    {
        if (i < 0 || i >= Dimension)
            throw new ArgumentOutOfRangeException(nameof(i));
    }

    #endregion Private Methods
}