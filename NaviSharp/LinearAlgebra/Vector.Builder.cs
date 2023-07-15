// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Numerics;

namespace NaviSharp;

public partial class Vector<T> where T : struct, IFloatingPoint<T>
{
    #region Public Methods

    public static Vector<T> FromVectorArray(params Vector<T>[] vectors)
    {
        var result = new Vector<T>(vectors.Sum(x => x.Dimension));
        var index = 0;
        foreach (var vector in vectors)
        {
            foreach (var element in vector)
            {
                result.At(index++, element);
            }
        }
        return result;
    }

    public static Vector<T> SubVector(Vector<T> vector, int startIndex, int dimension)
        => vector.SubVector(startIndex, dimension);

    public static Vector<T> SubVector(Vector<T> vector, int dimension)
        => vector.SubVector(dimension);

    #endregion Public Methods
}