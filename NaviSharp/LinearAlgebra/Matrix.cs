// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using NaviSharp.LinearAlgebra;

namespace NaviSharp;

#pragma warning disable CA2260

public partial class Matrix<T> :
MatrixVectorBase<Matrix<T>, T>,
IMultiplyOperators<Matrix<T>, Matrix<T>, Matrix<T>>
where T : struct, IFloatingPoint<T>
{
    #region Public Constructors

    static Matrix()
    {
        Empty = new();
    }

    public Matrix() : base()
    {
    }

    public Matrix(Matrix<T> matrix) : base(matrix._data, MatrixVectorConstructMode.Copy)
    {
        if (matrix == null)
            throw new ArgumentNullException(nameof(matrix));
        RowCount = matrix.RowCount;
        ColumnCount = matrix.ColumnCount;
    }

    public Matrix(int row, int column) : base(row * column)
    {
        RowCount = row;
        ColumnCount = column;
    }

    public Matrix(int row, int column, T num) : base(row * column, num)
    {
        RowCount = row;
        ColumnCount = column;
    }

    public Matrix(int row, int column, T[] nums, MatrixVectorConstructMode mode = MatrixVectorConstructMode.Ref) : base(nums, mode)
    {
        if (row * column != nums.Length)
            throw new ArgumentException($"The array length is not equal to the product of the number of rows and columns.It should be{row * column}");
        RowCount = row;
        ColumnCount = column;
    }

    public Matrix(T[,] nums) : base(nums)
    {
        RowCount = nums.GetLength(0);
        ColumnCount = nums.GetLength(1);
    }

    #endregion Public Constructors

    #region Public Enums

    public enum IndexMode
    {
        Row,
        Column
    }

    #endregion Public Enums

    #region Public Properties

    public static Matrix<T> Empty { get; }
    public int ColumnCount { get; init; }
    public bool IsSquare => RowCount == ColumnCount;
    public int RowCount { get; init; }

    #endregion Public Properties

    #region Public Indexers        
    public T this[int i, int j]
    {
        get
        {
            ValidateRange(i, j);
            return At(i, j);
        }
        set
        {
            ValidateRange(i, j);
            At(i, j, value);
        }
    }

    public T[] this[int index, IndexMode mode = IndexMode.Row]
    {
        get
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            switch (mode)
            {
                case Matrix<T>.IndexMode.Row:
                    if (index >= RowCount)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return GetRow(index);

                case Matrix<T>.IndexMode.Column:
                    if (index >= ColumnCount)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return GetColumn(index);

                default:
                    return GetRow(index);
            }
        }
        set
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            switch (mode)
            {
                case Matrix<T>.IndexMode.Row:
                    if (index >= RowCount)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    SetRow(index, value);
                    break;

                case Matrix<T>.IndexMode.Column:
                    if (index >= ColumnCount)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    SetColumn(index, value);
                    break;

                default:
                    SetRow(index, value);
                    break;
            }
        }
    }

    public Matrix<T> this[Range? rowRange = null, Range? columnRange = null]
    {
        get
        {
            var startRow = 0;
            var startColumn = 0;
            var rowCount = RowCount;
            var columnCount = ColumnCount;
            if (rowRange.HasValue)
                (startRow, rowCount) = rowRange.Value.GetOffsetAndLength(RowCount);
            if (columnRange.HasValue)
                (startColumn, columnCount) = columnRange.Value.GetOffsetAndLength(ColumnCount);
            return SubMatrix(startRow, startColumn, rowCount, columnCount);
        }
        set
        {
            var startRow = 0;
            var startColumn = 0;
            var rowCount = RowCount;
            var columnCount = ColumnCount;
            if (rowRange.HasValue)
                (startRow, rowCount) = rowRange.Value.GetOffsetAndLength(RowCount);
            if (columnRange.HasValue)
                (startColumn, columnCount) = columnRange.Value.GetOffsetAndLength(ColumnCount);
            SetRange(value, 0, 0, startRow, startColumn, rowCount, columnCount);
        }
    }

    #endregion Public Indexers

    #region Public Methods

    public static Matrix<T> Identity(int order)
    {
        var result = new Matrix<T>(order, order);
        for (int i = 0; i < order; i++)
        {
            result.At(i, i, T.One);
        }
        return result;
    }

    public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
    {
        if (left.ColumnCount != right.RowCount)
            throw new ArgumentException("The number of columns in the left matrix is not equal to the number of rows in the right matrix");
        return new Matrix<T>(left.RowCount, right.ColumnCount, DoMult(left._data, right._data, left.RowCount, right.ColumnCount, left.ColumnCount));
    }

    public static Matrix<T> operator *(T left, Matrix<T> right) => right * left;

    public static Matrix<T> Random(int row, int column, T min, T max)
    {
        var result = new Matrix<T>(row, column);
        result.AssignRandom(min, max);
        return result;
    }

    public Matrix<T> At(int i, int j, T num)
    {
        _data[i * ColumnCount + j] = num;
        return this;
    }

    public T At(int i, int j)
    {
        return _data[i * ColumnCount + j];
    }

    public Matrix<T> Combine(Matrix<T> other, MatrixCombinationMode mode)
    {
        return Matrix<T>.Combine(this, other, mode);
    }

    public T[] GetColumn(int j)
    {
        var result = new T[RowCount];
        int k = j;
        for (int i = 0; i < RowCount; i++)
        {
            result[i] = _data[k];
            k += ColumnCount;
        }
        return result;
    }

    public T[] GetDiagonal()
    {
        var length = Min(RowCount, ColumnCount);
        var result = new T[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = At(i, i);
        }
        return result;
    }

    public T[,] GetRange(int startRow, int startColumn, int rowCount, int columnCount)
    {
        ValidateRange(startRow, startColumn);
        ValidateRange(startRow + rowCount - 1, startColumn + columnCount - 1);
        var matrix = new T[rowCount, columnCount];
        for (int i = 0; i < rowCount; i++)
            for (int j = 0; j < columnCount; j++)
                matrix[i, j] = At(startRow + i, startColumn + j);
        return matrix;
    }

    public T[] GetRow(int i)
    {
        var result = new T[ColumnCount];
        Array.Copy(_data, i * ColumnCount, result, 0, ColumnCount);
        return result;
    }

    public Matrix<T> Inverse()
    {
        if (!IsSquare)
            throw new ArgumentException("Matrix must be square");
        switch (RowCount)
        {
            case 0: return new Matrix<T>();
            case 1:
                if (T.IsZero(_data[0]))
                    throw new ArgumentException("The matrix must be nonsingular");
                return new Matrix<T>(1, 1, T.One / _data[0]);

            case 2:
                var det = _data[0] * _data[3] - _data[1] * _data[2];
                if (det == T.Zero)
                    throw new ArgumentException("The matrix must be nonsingular");
                return new Matrix<T>(2, 2, new T[4] { _data[3] / det, -_data[1] / det, -_data[2] / det, _data[0] / det });

            default: return Inverse(this);
        }
    }

    public bool IsSizeOf(Matrix<T> other)
    {
        return IsSizeOf(other.RowCount, other.ColumnCount);
    }

    public bool IsSizeOf(int row, int column)
    {
        return RowCount == row && ColumnCount == column;
    }

    public Matrix<T> SetColumn(int j, params T[] nums)
    {
        int k = j;
        for (int i = 0; i < RowCount; i++)
        {
            _data[k] = nums[i];
            k += ColumnCount;
        }
        return this;
    }

    public Matrix<T> SetDiagonal(params T[] nums)
    {
        var length = Min(RowCount, ColumnCount);
        for (int i = 0; i < length; i++)
        {
            At(i, i, nums[i]);
        }
        return this;
    }

    public Matrix<T> SetRange(T[,] sourceMatrix, int sourceRow, int sourceColumn, int destinationRow, int destinationColumn, int rowCount, int columnCount)
    {
        ValidateRange(destinationRow, destinationColumn);
        ValidateRange(destinationRow + rowCount - 1, destinationColumn + columnCount - 1);
        try
        {
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < columnCount; j++)
                    At(i + destinationRow, j + destinationColumn, sourceMatrix[i + sourceRow, j + sourceColumn]);
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new ArgumentOutOfRangeException($"{nameof(sourceMatrix)} is out of range.", e);
        }
        return this;
    }

    public Matrix<T> SetRange(Matrix<T> sourceMatrix, int sourceRow, int sourceColumn, int destinationRow, int destinationColumn, int rowCount, int columnCount)
    {
        ValidateRange(destinationRow, destinationColumn);
        sourceMatrix.ValidateRange(sourceRow, sourceColumn);
        ValidateRange(destinationRow + rowCount - 1, destinationColumn + columnCount - 1);
        sourceMatrix.ValidateRange(sourceRow + rowCount - 1, sourceColumn + columnCount - 1);
        for (int i = 0; i < rowCount; i++)
            for (int j = 0; j < columnCount; j++)
                At(i + destinationRow, j + destinationColumn, sourceMatrix.At(i + sourceRow, j + sourceColumn));
        return this;
    }

    public Matrix<T> SetRow(int i, params T[] nums)
    {
        Array.Copy(nums, 0, _data, i * ColumnCount, ColumnCount);
        return this;
    }

    public Matrix<T> SubMatrix(int startRow, int startColumn, int rowCount, int columnCount)
    {
        ValidateRange(startRow, startColumn);
        ValidateRange(startRow + rowCount - 1, startColumn + columnCount - 1);
        var matrix = new Matrix<T>(rowCount, columnCount);
        for (int i = 0; i < rowCount; i++)
            for (int j = 0; j < columnCount; j++)
                matrix.At(i, j, At(i + startRow, j + startColumn));
        return matrix;
    }

    public Matrix<T> SubMatrix(int rowCount, int columnCount) => SubMatrix(0, 0, rowCount, columnCount);

    public override sealed string ToString()
    {
        StringBuilder stringBuilder = new(Environment.NewLine);
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                stringBuilder.Append(string.Format("{0,8}", At(i, j).ToString("E4", null))).Append(' ');
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }

    public Matrix<T> Transpose()
    {
        var result = new Matrix<T>(ColumnCount, RowCount);
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                result.At(j, i, At(i, j));
            }
        }
        return result;
    }

    public T Trace()
    {
        if (!IsSquare)
            throw new InvalidOperationException("The trace only exist if the matrix is square.");
        var nums = GetDiagonal();
        var sum = T.Zero;
        for (int i = 0; i < RowCount; i++)
            sum += nums[i];
        return sum;
    }

    #endregion Public Methods

    #region Protected Methods

    protected static T[] DoMult(T[] left, T[] right, int leftRow, int rightColumn, int leftColumn)
    {
        var result = new T[leftRow * rightColumn];
        if (result.Length > 250000)
        {
            var partitioner = Partitioner.Create(0, leftRow);
            Parallel.ForEach(partitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    for (int k = 0; k < leftColumn; k++)
                        for (int j = 0; j < rightColumn; j++)
                            result[i * rightColumn + j] += left[i * leftColumn + k] * right[k * rightColumn + j];
            });
        }
        else
        {
            var v = new System.Numerics.Vector<double>();
            var vv = v.AsVector256();
            for (int i = 0; i < leftRow; i++)
                for (int k = 0; k < leftColumn; k++)
                    for (int j = 0; j < rightColumn; j++)
                        result[i * rightColumn + j] += left[i * leftColumn + k] * right[k * rightColumn + j];
        }
        return result;
    }

    #endregion Protected Methods

    #region Private Methods

    //private void RowMult(int i, T num)
    //{
    //    DoMult(num, _data, i * ColumnCount, ColumnCount, _data, i * ColumnCount);
    //}

    //private void RowMultAdd(int i1, T num, int i2)
    //{
    //    var temp = new T[ColumnCount];
    //    DoMult(num, _data, i2 * ColumnCount, ColumnCount, temp, 0);
    //    DoAdd(_data, i1 * ColumnCount, temp, 0, ColumnCount, _data, i1 * ColumnCount);
    //}

    private void SwapRow(int i1, int i2)
    {
        var k1 = i1 * ColumnCount;
        var k2 = i2 * ColumnCount;
        for (int j = 0; j < ColumnCount; j++)
        {
            k1 += j;
            k2 += j;
            (_data[k1], _data[k2]) = (_data[k2], _data[k1]);
        }
    }

    private void ValidateRange(int i, int j, [CallerArgumentExpression(nameof(i))] string? nameofI = null, [CallerArgumentExpression(nameof(j))] string? nameofJ = null)
    {
        if (i >= RowCount || i < 0)
        {
            throw new ArgumentOutOfRangeException(nameofI);
        }

        if (j >= ColumnCount || j < 0)
        {
            throw new ArgumentOutOfRangeException(nameofJ);
        }
    }

    #endregion Private Methods
}