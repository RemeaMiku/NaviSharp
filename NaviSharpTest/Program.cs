// See https://aka.ms/new-console-template for more information
using NaviSharp;
Console.WriteLine("NaviSharp");
Console.WriteLine(LeapSecond.GetLeapSeconds(new DateTimeOffset(1980, 1, 1, 0, 0, 0, TimeSpan.Zero)));