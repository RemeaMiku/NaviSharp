// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp;

public partial class Matrix<T>
{
    #region Private Methods

    private static Matrix<T> Decompose(Matrix<T> matrix, out int[] perm, out int toggle)
    {
        // Doolittle LUP decomposition with partial pivoting.
        // rerturns: result is L (with 1s on diagonal) and U;
        // perm holds row permutations; toggle is +1 or -1 (even or odd)
        int rows = matrix.RowCount;
        int cols = matrix.ColumnCount; // assume square
        if (rows != cols)
            throw new Exception("Attempt to decompose a non-square m");

        int n = rows; // convenience

        var result = new Matrix<T>(matrix);

        perm = new int[n]; // set up row permutation result
        for (int i = 0; i < n; ++i) { perm[i] = i; }

        toggle = 1; // toggle tracks row swaps.
                    // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

        for (int j = 0; j < n - 1; ++j) // each column
        {
            var colMax = System.Math.Abs((dynamic)result[j, j]); // find largest val in col
            int pRow = j;
            //for (int i = j + 1; i less-than n; ++i)
            //{
            //  if (result[i][j] greater-than colMax)
            //  {
            //    colMax = result[i][j];
            //    pRow = i;
            //  }
            //}

            // reader Matt V needed this:
            for (int i = j + 1; i < n; ++i)
            {
                if (System.Math.Abs((dynamic)result[i, j]) > colMax)
                {
                    colMax = System.Math.Abs((dynamic)result[i, j]);
                    pRow = i;
                }
            }
            // Not sure if this approach is needed always, or not.

            if (pRow != j) // if largest value not on pivot, swap rows
            {
                (result[j], result[pRow]) = (result[pRow], result[j]);
                (perm[j], perm[pRow]) = (perm[pRow], perm[j]); // and swap perm info
                toggle = -toggle; // adjust the row-swap toggle
            }

            // --------------------------------------------------
            // This part added later (not in original)
            // and replaces the 'return null' below.
            // if there is a 0 on the diagonal, find a good row
            // from i = j+1 down that doesn't have
            // a 0 in column j, and swap that good row with row j
            // --------------------------------------------------

            if (result[j, j] == T.Zero)
            {
                // find a good row to swap
                int goodRow = -1;
                for (int row = j + 1; row < n; ++row)
                {
                    if (result[row, j] != T.Zero)
                        goodRow = row;
                }

                if (goodRow == -1)
                    throw new Exception("Cannot use Doolittle's method");

                // swap rows so 0.0 no longer on diagonal
                (result[j], result[goodRow]) = (result[goodRow], result[j]);
                (perm[j], perm[goodRow]) = (perm[goodRow], perm[j]); // and swap perm info

                toggle = -toggle; // adjust the row-swap toggle
            }
            // --------------------------------------------------
            // if diagonal after swap is zero . .
            //if (Math.Abs(result[j][j]) less-than 1.0E-20)
            //  return null; // consider a throw

            for (int i = j + 1; i < n; ++i)
            {
                result[i, j] /= result[j, j];
                for (int k = j + 1; k < n; ++k)
                {
                    result[i, k] -= result[i, j] * result[j, k];
                }
            }
        } // main j column loop

        return result;
    }

    private static T[] HelperSolve(Matrix<T> luMatrix, T[] b)
    {
        // before calling this helper, permute b using the perm array
        // from MatrixDecompose that generated luMatrix
        int n = luMatrix.RowCount;
        var x = new T[n];
        b.CopyTo(x, 0);

        for (int i = 1; i < n; ++i)
        {
            T sum = x[i];
            for (int j = 0; j < i; ++j)
                sum -= luMatrix[i, j] * x[j];
            x[i] = sum;
        }

        x[n - 1] /= luMatrix[n - 1][n - 1];
        for (int i = n - 2; i >= 0; --i)
        {
            T sum = x[i];
            for (int j = i + 1; j < n; ++j)
                sum -= luMatrix[i, j] * x[j];
            x[i] = sum / luMatrix[i, i];
        }

        return x;
    }

    private static Matrix<T> Inverse(Matrix<T> matrix)
    {
        int n = matrix.RowCount;
        var result = new Matrix<T>(matrix);
        var lum = Decompose(matrix, out int[] perm,
          out _) ?? throw new Exception("Unable to compute inverse");
        var b = new T[n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (i == perm[j])
                    b[j] = T.One;
                else
                    b[j] = T.Zero;
            }

            var x = HelperSolve(lum, b);

            for (int j = 0; j < n; ++j)
                result[j, i] = x[j];
        }
        return result;
    }

    #endregion Private Methods
}