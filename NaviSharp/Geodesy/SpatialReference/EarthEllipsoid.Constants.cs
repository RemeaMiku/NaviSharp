// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

namespace NaviSharp.SpatialReference;

public partial record class EarthEllipsoid
{

    public readonly static EarthEllipsoid Cgcs2000
        = new(6378137.0, 6356752.31414036, "CGCS2000");
    public readonly static EarthEllipsoid Grs80
        = new(6378137.0, 6356752.31414036, "GRS80");
    public readonly static EarthEllipsoid Wgs84
        = new(6378137.0, 6356752.31424517, "WGS84");

}