// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp;

public partial record class EarthEllipsoid
{
    #region Public Classes

    public static class Cgcs2000Constants
    {
        #region Public Fields

        public const double a = 6378137.0;
        public const double b = 6356752.31414036;

        #endregion Public Fields
    }

    public static class Grs80Constants
    {
        #region Public Fields

        public const double a = 6378137.0;
        public const double b = 6356752.31414036;

        #endregion Public Fields
    }

    public static class Wgs84Constants
    {
        #region Public Fields

        public const double a = 6378137.0;
        public const double b = 6356752.31424517;

        #endregion Public Fields
    }
    #endregion Public Classes
}