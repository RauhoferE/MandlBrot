using NetworkLibraryMandelBrot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using WebApplication1.Helper;

namespace WebApplication1.Services
{
    public class MandelbrotService : IMandelbrotService
    {
        private const int RE_START = -2;
        private const int RE_END = 1;
        private const int IM_START = -1;
        private const int IM_END = 1;

        private ConcurrentBag<(Tuple<int, int>, Color)> colorArray = new ConcurrentBag<(Tuple<int, int>, Color)>();
        private ILogger logger;
        private static bool isComputed = false;
        private static object lk = new object();
        public MandelbrotService(ILogger<MandelbrotService> logger)
        {
            this.logger = logger;
        }


        public async Task<Bitmap> GetMandelBrotBitmap(MandelBrotRequest request)
        {
            Bitmap bm = new Bitmap(request.Width, request.Height);

            int p2 = request.Height / 4;
            int startHeight = 0;
            for (int i = 0; i < 4; i++)
            {
                await CalculateMandelbrotColor(0, startHeight, request.Width, startHeight + p2, request.Iteration, request.Height);
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
                    logger.LogError(e.Message);
                    return null;
                }
            }

            return bm;
        }

        private async Task<bool> CalculateMandelbrotColor(int startWidth, int startHeight, int width, int height, int iteration, int realHight)
        {
            for (int k = startHeight; k < height; k++)
            {

                for (int i = startWidth; i < width; i++)
                {
                    // Threadpool für jeden Punkt
                    // Mit Countdown
                    Complex c = new Complex(RE_START + (Convert.ToDouble(i) / Convert.ToDouble(width)) * (RE_END - RE_START),
                    IM_START + (Convert.ToDouble(k) / Convert.ToDouble(realHight)) * (IM_END - IM_START));

                    int m = CalculateMandelbrot(iteration, c);

                    var colorInt = 255 - (m * 255 / iteration);
                    colorArray.Add((new Tuple<int, int>(i, k), ColorHelper.Colors[m % ColorHelper.Colors.Count()]));
                }
            }

            SetBool(realHight * width);

            return true;
        }

        private void SetBool(int colletionCount)
        {
            if (colorArray.Count == colletionCount)
            {
                lock (lk)
                {
                    isComputed = true;
                }
            }
        }

        private int CalculateMandelbrot(int iteration, Complex c)
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
    }
}
