using Microsoft.Extensions.Logging;
using NetworkLibraryMandelBrot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Helper;

namespace WebApplication1.Services
{
    public class MandelbrotServiceV2 : IMandelbrotService
    {
        private const int RE_START = -2;
        private const int RE_END = 1;
        private const int IM_START = -1;
        private const int IM_END = 1;

        private ConcurrentBag<(Tuple<int, int>, Color)> colorArray = new ConcurrentBag<(Tuple<int, int>, Color)>();
        private ILogger logger;
        private CountdownEvent countdown;
        public MandelbrotServiceV2(ILogger<MandelbrotService> logger)
        {
            this.logger = logger;
        }


        public async Task<Bitmap> GetMandelBrotBitmap(MandelBrotRequest request)
        {
            Bitmap bm = new Bitmap(request.Width, request.Height);

            CalculateMandelbrotColor(request.Width, request.Height, request.Iteration);

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

        private async Task<bool> CalculateMandelbrotColor(int width, int height, int iteration)
        {
            countdown = new CountdownEvent(width * height);

            for (int k = 0; k < height; k++)
            {

                for (int i = 0; i < width; i++)
                {
                    var x = i + 0;
                    var y = k + 0;
                    Complex c = new Complex(RE_START + (Convert.ToDouble(i) / Convert.ToDouble(width)) * (RE_END - RE_START),
                    IM_START + (Convert.ToDouble(k) / Convert.ToDouble(height)) * (IM_END - IM_START));

                    ThreadPool.QueueUserWorkItem(_ => CalculateMandelbrot(iteration, c, x,y));
                }
            }

            countdown.Wait();
            countdown.Dispose();

            return true;
        }

        private void CalculateMandelbrot(int iteration, Complex c, int x, int y)
        {
            Complex z = 0;
            int n = 0;

            while ((Complex.Abs(z) <= 2) && (n < iteration))
            {
                z = Complex.Add((z * z), c);
                n = n + 1;
            }

            //var colorInt = 255 - (n * 255 / iteration);
            colorArray.Add((new Tuple<int, int>(x, y), ColorHelper.Colors[n % ColorHelper.Colors.Count()]));
            countdown.Signal();
        }
    }
}
