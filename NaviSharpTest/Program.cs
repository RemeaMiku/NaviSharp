// See https://aka.ms/new-console-template for more information
using System.Numerics;
using NaviSharp;
using NaviSharp.Orientation;
using NaviSharp.SpatialReference;

Console.WriteLine("NaviSharp");
var e = new EulerAngles(Angle.FromDegrees(183.1), Angle.FromDegrees(-39), Angle.FromDegrees(-83.1));
Console.WriteLine(e.ToRotationVector().ToQuaternion().ToRotationMatrix().ToEulerAngles().ToRotationMatrix().ToRotationVector().ToEulerAngles().ToQuaternion().ToRotationVector().ToQuaternion().ToEulerAngles());