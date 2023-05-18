// See https://aka.ms/new-console-template for more information
using NaviSharp;
Console.WriteLine("NaviSharp");
var test = new[] { 0, 1, 2, 3, 4, 5 };
var test1 = test[1..^2];
var mat = new Matrix<double>(new[,]
{
    {1d,2,3 },
    {4,5,6 },
    {7,8,9 },
});
var range = 1..3;
Console.WriteLine(test1.ToString());