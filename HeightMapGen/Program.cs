#region Copyright

// COPYRIGHT 2015 JUSTIN COX (CONJI)

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace HeightMapGen
{
    internal class Program
    {
        private static double[] _values;
        private static readonly Random _random = new Random();
        private static Bitmap _result;

        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Incorrect syntax. Use hmg STEPSIZE FEATURESIZE");
                return;
            }
            var featureSize = int.Parse(args[1]);
            var sampleSize = featureSize;
            var scale = 1.0d;
            var stepSize = int.Parse(args[0]);
            _values = new double[stepSize*stepSize];
            Console.WriteLine("Generating terrain. Please wait...");
            for (var y = 0; y < featureSize; y++)
            {
                for (var x = 0; x < featureSize; x++)
                {
                    SetSample(x, y, featureSize, Frand());
                }
            }
            while (sampleSize > 1)
            {
                DiamondSquare(stepSize, scale);
                sampleSize /= 2;
                scale /= 2.0;
            }
            var result = new Bitmap(featureSize, featureSize);
            for (var y = 0; y < featureSize; y++)
            {
                for (var x = 0; x < featureSize; x++)
                {
                    var d = (int) (Sample(x, y, featureSize)*255);
                    if (d > 200) d = 200; // highest point allowed
                    if (d < 100) d = 100; // lowest point allowed
                    result.SetPixel(x, y, Color.FromArgb(d, d, d));
                }
            }
            result.Save("result.jpg", ImageFormat.Jpeg);
            Console.WriteLine("Completed");

            Console.ReadLine();
        }

        private static void DiamondSquare(int stepsize, double scale)
        {
            var halfstep = stepsize/2;

            for (var y = halfstep; y < 128 + halfstep; y += stepsize)
            {
                for (var x = halfstep; x < 128 + halfstep; x += stepsize)
                {
                    SampleSquare(x, y, stepsize, Frand()*scale);
                }
            }

            for (var y = 0; y < 128; y += stepsize)
            {
                for (var x = 0; x < 128; x += stepsize)
                {
                    SampleDiamond(x + halfstep, y, stepsize, Frand()*scale);
                    SampleDiamond(x, y + halfstep, stepsize, Frand()*scale);
                }
            }
        }

        private static void SampleSquare(int x, int y, int size, double value)
        {
            var hs = size/2;
            var a = Sample(x - hs, y - hs, size);
            var b = Sample(x + hs, y - hs, size);
            var c = Sample(x - hs, y + hs, size);
            var d = Sample(x + hs, y + hs, size);

            SetSample(x, y, size, ((a + b + c + d)/4.0) + value);
        }

        private static void SampleDiamond(int x, int y, int size, double value)
        {
            var hs = size/2;
            var a = Sample(x - hs, y, size);
            var b = Sample(x + hs, y, size);
            var c = Sample(x, y - hs, size);
            var d = Sample(x, y + hs, size);

            SetSample(x, y, size, ((a + b + c + d)/4.0) + value);
        }

        private static double Sample(int x, int y, int size)
        {
            return _values[(x & (size - 1)) + (y & (size - 1))*size];
        }

        private static void SetSample(int x, int y, int size, double value)
        {
            _values[(x & (size - 1)) + (y & (size - 1))*size] = value;
        }

        private static double Frand()
        {
            var b = _random.Next(2) == 1;
            var d = _random.NextDouble();
            if (b) d = -d;
            return d;
        }
    }
}