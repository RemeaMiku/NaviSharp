// See https://aka.ms/new-console-template for more information
using NaviSharp;
Console.WriteLine("NaviSharp");
Console.WriteLine(new GeodeticCoord(Angle.FromDegrees(40.08226974784), Angle.FromDegrees(116.24194286667), 63.5237).ToCart(EarthEllipsoid.Wgs84));