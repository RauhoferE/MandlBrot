using NetworkLibraryMandelBrot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace TESTINGSTUFF
{
    class Program
    {

        private const int RE_START = -2;
        private const int RE_END = 1;
        private const int IM_START = -1;
        private const int IM_END = 1;

        private static ConcurrentBag<(Tuple<int, int>, Color)> colorArray = new ConcurrentBag<(Tuple<int, int>, Color)>();



        private static bool isComputed = false;
        private static object lk = new object();
        static void Main(string[] args)
        {
            MandelBrotRequest mbr = new MandelBrotRequest();
            mbr.Height = 400;
            mbr.Width = 600;
            mbr.Iteration = 80;
            GetMandelBrotBitmap(mbr);
        }

        public static async Task<Bitmap> GetMandelBrotBitmap(MandelBrotRequest request)
        {
            Bitmap bm = new Bitmap(request.Width, request.Height);

            //int p1 = request.Width / 2;
            int p2 = request.Height / 4;
            //int startWidth = 0;
            int startHeight = 0;
            for (int i = 0; i < 4; i++)
            {
                await CalculateMandelbrotColor(0, startHeight, request.Width, startHeight + p2, request.Iteration, request.Height);
                //startWidth = startWidth + p1;
                startHeight = startHeight + p2;
            }

            while (!isComputed)
            {
                Thread.Sleep(10);
            }

            foreach (var colorT in colorArray)
            {
                try
                {
                    bm.SetPixel(colorT.Item1.Item1, colorT.Item1.Item2, colorT.Item2);
                }
                catch (Exception e)
                {
                    Console.Write("x " + colorT.Item1.Item1 + " y " + colorT.Item1.Item2);
                    Console.WriteLine(e);
                }
            }

            bm.Save("./pic.png", ImageFormat.Png);
            return bm;
        }

        private static async Task<bool> CalculateMandelbrotColor(int startWidth, int startHeight, int width, int height, int iteration, int realHight)
        {
            for (int k = startHeight; k < height; k++)
            {

                for (int i = startWidth; i < width; i++)
                {
                    Complex c = new Complex(RE_START + (Convert.ToDouble(i) / Convert.ToDouble(width)) * (RE_END - RE_START),
                    IM_START + (Convert.ToDouble(k) / Convert.ToDouble(realHight)) * (IM_END - IM_START));

                    int m = CalculateMandelbrot(iteration, c);

                    int hue = 255 * m / iteration;
                    int v;
                    if (m > iteration)
                    {
                        v = 255;
                    }
                    else
                    {
                        v = 0;
                    }
                    var colorInt = 255 - (m * 255 / iteration);

                    colorArray.Add((new Tuple<int, int>(i, k), Color.FromArgb(hue, 255, v)));
                }
            }

            SetBool(realHight * width);

            return true;
        }

        private static int CalculateMandelbrot(int iteration, Complex c)
        {
            Complex z = 0;
            int n = 0;

            while ((Complex.Abs(z) <= 2) && (n < iteration))
            {
                z = Complex.Add((z * z), c);
                n = n + 1;
            }
            return n;
        }

        private static void SetBool(int colletionCount)
        {
            if (colorArray.Count == colletionCount)
            {
                lock (lk)
                {
                    isComputed = true;
                }
            }
        }
    }
}
