// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp.Geodesy.MapProjection;

public interface IMapProjection
{
    #region Public Methods

    public (double X, double Y) ToPlane(Angle latitude, Angle longitude);
    public (Angle Latitude, Angle Longitude) ToEllipsoid(double x, double y);

    #endregion Public Methods
}
