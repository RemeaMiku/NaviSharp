// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Collections;
using System.Numerics;

namespace NaviSharp;

public enum MatrixVectorConstructMode
{
    Ref,
    Copy
}

public abstract class MatrixVectorBase<M, T> :
    ICloneable,
    IEnumerable<T>,
    IAdditionOperators<M, MatrixVectorBase<M, T>, M>,
    ISubtractionOperators<M, MatrixVectorBase<M, T>, M>,
    IMultiplyOperators<MatrixVectorBase<M, T>, T, M>,
    IDivisionOperators<MatrixVectorBase<M, T>, T, M>,
    IUnaryNegationOperators<MatrixVectorBase<M, T>, M>
    where M : MatrixVectorBase<M, T>, new()
    where T : struct, IFloatingPoint<T>
{
    #region Public Constructors

    public MatrixVectorBase()
    {
        _data = Array.Empty<T>();
    }

    public MatrixVectorBase(int count)
    {
        _data = new T[count];
    }

    public MatrixVectorBase(int count, T num)
    {
        _data = new T[count];
        for (int i = 0; i < count; i++)
        {
            _data[i] = num;
        }
    }

    public MatrixVectorBase(T[] data, MatrixVectorConstructMode mode = MatrixVectorConstructMode.Ref)
    {
        switch (mode)
        {
            case MatrixVectorConstructMode.Copy:
                _data = new T[data.Length];
                Array.Copy(data, _data, Count);
                break;

            default:
                _data = data;
                break;
        }
    }

    public MatrixVectorBase(T[,] data)
    {
        _data = new T[data.GetLength(0) * data.GetLength(1)];
        int i = 0;
        foreach (var item in data)
        {
            _data[i++] = item;
        }
    }

    #endregion Public Constructors

    #region Public Properties

    public int Count => _data.Length;

    public T[] Data => _data;

    #endregion Public Properties

    #region Public Methods

    public static M operator -(M left, MatrixVectorBase<M, T> right)
    {
        if (left is Matrix<T> leftMat && right is Matrix<T> rightMat)
        {
            if (!leftMat.IsSizeOf(rightMat))
                throw new ArgumentException("The number of rows and columns of the left and right matrices is not equal");
            var result = new Matrix<T>(leftMat.RowCount, leftMat.ColumnCount);
            DoSub(left._data, right._data, result._data);
            return (dynamic)result;
        }
        if (left is Vector<T> leftVec && right is Vector<T> rightVec)
        {
            if (!leftVec.IsSizeOf(rightVec))
                throw new ArgumentException("The dimensions of left and right vectors are not equal");
            var result = new Vector<T>(leftVec.Dimension, leftVec.IsColumn);
            DoSub(left._data, right._data, result._data);
            return (dynamic)result;
        }
        throw new NotSupportedException();
    }

    public static M operator -(MatrixVectorBase<M, T> value)
    {
        if (value is Matrix<T> matrix)
            return (dynamic)(-T.One * matrix);
        if (value is Vector<T> vector)
            return (dynamic)(-T.One * vector);
        throw new NotSupportedException();
    }

    public static M operator *(MatrixVectorBase<M, T> left, T right)
    {
        if (left is Matrix<T> leftMat)
        {
            var result = new Matrix<T>(leftMat.RowCount, leftMat.ColumnCount);
            DoMult(right, left._data, result._data);
            return (dynamic)result;
        }
        if (left is Vector<T> leftVec)
        {
            var result = new Vector<T>(leftVec.Dimension, leftVec.IsColumn);
            DoMult(right, left._data, result._data);
            return (dynamic)result;
        }
        throw new NotSupportedException();
    }

    public static M operator /(MatrixVectorBase<M, T> left, T right)
    {
        if (left is Matrix<T> leftMat)
        {
            var result = new Matrix<T>(leftMat.RowCount, leftMat.ColumnCount);
            DoMult(T.One / right, left._data, result._data);
            return (dynamic)result;
        }
        if (left is Vector<T> leftVec)
        {
            var result = new Vector<T>(leftVec.Dimension, leftVec.IsColumn);
            DoMult(T.One / right, left._data, result._data);
            return (dynamic)result;
        }
        throw new NotSupportedException();
    }

    public static M operator +(M left, MatrixVectorBase<M, T> right)
    {
        if (left is Matrix<T> leftMat && right is Matrix<T> rightMat)
        {
            if (!leftMat.IsSizeOf(rightMat))
                throw new ArgumentException("The number of rows and columns of the left and right matrices is not equal");
            var result = new Matrix<T>(leftMat.RowCount, leftMat.ColumnCount);
            DoAdd(left._data, right._data, result._data);
            return (dynamic)result;
        }
        if (left is Vector<T> leftVec && right is Vector<T> rightVec)
        {
            if (!leftVec.IsSizeOf(rightVec))
                throw new ArgumentException("The dimensions of left and right vectors are not equal");
            var result = new Vector<T>(leftVec.Dimension, leftVec.IsColumn);
            DoAdd(left._data, right._data, result._data);
            return (dynamic)result;
        }
        throw new NotSupportedException();
    }

    public object Clone()
    {
        if (this is Matrix<T> matrix)
            return new Matrix<T>(matrix.RowCount, matrix.ColumnCount, _data, MatrixVectorConstructMode.Copy);
        throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    #endregion Public Methods

    #region Protected Fields

    protected readonly T[] _data;

    #endregion Protected Fields

    #region Protected Methods

    protected static void DoAdd(T[] left, int leftStart, T[] right, int rightStart, int length, T[] result, int resultStart)
    {
        var vectorSize = System.Numerics.Vector<T>.Count;
        int i;
        for (i = 0; i <= length - vectorSize; i += vectorSize)
        {
            var vector1 = new System.Numerics.Vector<T>(left, leftStart + i);
            var vector2 = new System.Numerics.Vector<T>(right, rightStart + i);
            (vector1 + vector2).CopyTo(result, resultStart + i);
        }
        for (; i < length; i++)
        {
            result[resultStart + i] = left[leftStart + i] + right[rightStart + i];
        }
    }

    protected static void DoAdd(T[] left, T[] right, T[] result)
    {
        DoAdd(left, 0, right, 0, left.Length, result, 0);
    }

    protected static T DoDot(T[] left, int leftStart, T[] right, int rightStart, int length)
    {
        var vectorSize = System.Numerics.Vector<T>.Count;
        T result = T.Zero;
        int i;
        for (i = 0; i <= length - vectorSize; i += vectorSize)
        {
            var vector1 = new System.Numerics.Vector<T>(left, leftStart + i);
            var vector2 = new System.Numerics.Vector<T>(right, rightStart + i);
            result += Vector.Dot(vector1, vector2);
        }
        for (; i < length; i++)
        {
            result += left[leftStart + i] * right[rightStart + i];
        }
        return result;
    }

    protected static T DoDot(T[] left, T[] right)
    {
        return DoDot(left, 0, right, 0, left.Length);
    }

    protected static void DoMult(T num, T[] nums, int numsStart, int length, T[] result, int resultStart)
    {
        var vectorSize = System.Numerics.Vector<T>.Count;
        int i;
        for (i = 0; i <= length - vectorSize; i += vectorSize)
        {
            var vector = new System.Numerics.Vector<T>(nums, numsStart + i);
            (num * vector).CopyTo(result, resultStart + i);
        }
        for (; i < length; i++)
        {
            result[resultStart + i] = num * nums[numsStart + i];
        }
    }

    protected static void DoMult(T num, T[] nums, T[] result)
    {
        DoMult(num, nums, 0, nums.Length, result, 0);
    }

    protected static void DoSub(T[] left, int leftStart, T[] right, int rightStart, int length, T[] result, int resultStart)
    {
        var vectorSize = System.Numerics.Vector<T>.Count;
        int i;
        for (i = 0; i <= length - vectorSize; i += vectorSize)
        {
            var vector1 = new System.Numerics.Vector<T>(left, leftStart + i);
            var vector2 = new System.Numerics.Vector<T>(right, rightStart + i);
            (vector1 - vector2).CopyTo(result, resultStart + i);
        }
        for (; i < length; i++)
        {
            result[resultStart + i] = left[leftStart + i] - right[rightStart + i];
        }
    }

    protected static void DoSub(T[] left, T[] right, T[] result)
    {
        DoSub(left, 0, right, 0, left.Length, result, 0);
    }

    protected void AssignRandom()
    {
        var r = new Random();
        for (int i = 0; i < Count; i++)
        {
            _data[i] = (T)(dynamic)r.Next(100);
        }
    }

    #endregion Protected Methods
}