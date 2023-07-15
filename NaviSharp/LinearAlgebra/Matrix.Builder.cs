// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp;

public enum MatrixCombinationMode
{
    Vertical,
    Horizontal,
    Diagonal
}

public partial class Matrix<T>
{
    #region Public Methods

    public static Matrix<T> FromVectorsAsRows(params Vector<T>[] vectors)
    {
        var column = vectors[0].Dimension;
        if (!vectors.All(vector => vector.Dimension == column))
            throw new ArgumentException("The vectors in the array must have the same dimension.");
        var matrix = new Matrix<T>(vectors.Length, column);
        for (var i = 0; i < matrix.RowCount; i++)
            for (var j = 0; j < matrix.ColumnCount; j++)
                matrix.At(i, j, vectors[i].At(j));
        return matrix;
    }

    public static Matrix<T> FromVectorsAsColumns(params Vector<T>[] vectors)
    {
        var row = vectors[0].Dimension;
        if (!vectors.All(vector => vector.Dimension == row))
            throw new ArgumentException("The vectors in the array must have the same dimension");
        var matrix = new Matrix<T>(row, vectors.Length);
        for (var i = 0; i < matrix.RowCount; i++)
            for (var j = 0; j < matrix.ColumnCount; j++)
                matrix.At(i, j, vectors[j].At(i));
        return matrix;
    }

    public static Matrix<T> Combine(Matrix<T> matrix1, Matrix<T> matrix2, MatrixCombinationMode mode = MatrixCombinationMode.Horizontal)
    {
        switch (mode)
        {
            case MatrixCombinationMode.Vertical:
                {
                    if (matrix1.ColumnCount != matrix2.ColumnCount)
                        throw new ArgumentException("The number of columns in the upper and lower matrices is not equal");
                    var result = new Matrix<T>(matrix1.RowCount + matrix2.RowCount, matrix1.ColumnCount);
                    for (int i = 0; i < matrix1.RowCount; i++)
                    {
                        result.SetRow(i, matrix1.GetRow(i));
                    }
                    for (int i = 0; i < matrix2.RowCount; i++)
                    {
                        result.SetRow(i + matrix1.RowCount, matrix2.GetRow(i));
                    }
                    return result;
                }
            case MatrixCombinationMode.Horizontal:
                {
                    if (matrix1.RowCount != matrix2.RowCount)
                        throw new ArgumentException("The number of rows in the left and right matrices is not equal");
                    var result = new Matrix<T>(matrix1.RowCount, matrix1.ColumnCount + matrix2.ColumnCount);
                    for (int j = 0; j < matrix1.ColumnCount; j++)
                    {
                        result.SetColumn(j, matrix1.GetColumn(j));
                    }
                    for (int j = 0; j < matrix2.ColumnCount; j++)
                    {
                        result.SetColumn(j + matrix1.ColumnCount, matrix2.GetColumn(j));
                    }
                    return result;
                }
            case MatrixCombinationMode.Diagonal:
                {
                    var result = new Matrix<T>(matrix1.RowCount + matrix2.RowCount, matrix1.ColumnCount + matrix2.ColumnCount);
                    for (int i = 0; i < matrix1.RowCount; i++)
                    {
                        for (int j = 0; j < matrix1.ColumnCount; j++)
                        {
                            result.At(i, j, matrix1.At(i, j));
                        }
                    }
                    for (int i = 0; i < matrix2.RowCount; i++)
                    {
                        for (int j = 0; j < matrix2.ColumnCount; j++)
                        {
                            result.At(i + matrix1.RowCount, j + matrix1.ColumnCount, matrix2.At(i, j));
                        }
                    }
                    return result;
                }
            default:
                throw new NotSupportedException();
        }
    }

    public static Matrix<T> FromArrayAsDiagonal(params T[] values)
    {
        var result = new Matrix<T>(values.Length, values.Length);
        result.SetDiagonal(values);
        return result;
    }

    public static Matrix<T> FromCrossProduct(Vector<T> vector)
    {
        if (!vector.IsSizeOf(3))
            throw new ArgumentException("");
        return new(3, 3, new[]
        {
            T.Zero, -vector.At(2),vector.At(1),
            vector.At(2),T.Zero,-vector.At(0),
            -vector.At(1),vector.At(0),T.Zero
        });
    }

    public static Matrix<T> FromBlockMatrixArray(Matrix<T>[,] array)
    {
        var blockRow = array[0, 0].RowCount;
        var blockColumn = array[0, 0].ColumnCount;
        foreach (var mat in array)
        {
            if (!mat.IsSizeOf(blockRow, blockColumn))
                throw new ArgumentException("The matrices in the array are not equal in size");
        }
        var arrayRow = array.GetLength(0);
        var arrayColumn = array.GetLength(1);
        var result = new Matrix<T>(blockRow * arrayRow, blockColumn * arrayColumn);
        var arrayRowIndex = 0;
        var arrayColumnIndex = 0;
        var matRowIndex = 0;
        var matColumnIndex = 0;
        for (int resRowIndex = 0; resRowIndex < result.RowCount; resRowIndex++)
        {
            for (int resColumnIndex = 0; resColumnIndex < result.ColumnCount; resColumnIndex++)
            {
                result.At(resRowIndex, resColumnIndex, array[arrayRowIndex, arrayColumnIndex].At(matRowIndex, matColumnIndex));
                arrayColumnIndex = (resColumnIndex + 1) / blockColumn;
                matColumnIndex++;
                matColumnIndex %= blockColumn;
            }
            arrayRowIndex = (resRowIndex + 1) / blockRow;
            matRowIndex++;
            matRowIndex %= blockRow;
            arrayColumnIndex = 0;
        }
        return result;
    }

    public static Matrix<T> FromVectorAsDiagonal(Vector<T> vector)
    {
        var result = new Matrix<T>(vector.Dimension, vector.Dimension);
        result.SetDiagonal(vector.Data);
        return result;
    }

    #endregion Public Methods
}