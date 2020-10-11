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
        public MandelbrotService(ILogger<MandelbrotService> logger)
        {
            this.logger = logger;
        }


        public async Task<Bitmap> GetMandelBrotBitmap(MandelBrotRequest request)
        {
            List<Task<bool>> boolTaskList = new List<Task<bool>>();
            Bitmap bm = new Bitmap(request.Width, request.Height);

            int p1 = request.Width / 4;
            int p2 = request.Height / 4;
            int startWidth = 0;
            int startHeight = 0;
            for (int i = 0; i < 4; i++)
            {
                await CalculateMandelbrotColor(startWidth, startHeight, startWidth + p1, startHeight + p2, request.Iteration);
                startWidth += p1;
                startHeight += p2;
            }

            //foreach (var item in boolTaskList)
            //{
            //    var temp = await item;
            //    if (temp == true)
            //    {
            //        logger.LogInformation("Success");
            //    }
            //}

            foreach (var colorT in colorArray)
            {
                try
                {
                    bm.SetPixel(colorT.Item1.Item1, colorT.Item1.Item2, colorT.Item2);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            bm.Save("./pic.png", ImageFormat.Png);
            return bm;
        }

        private async Task<bool> CalculateMandelbrotColor(int startWidth, int startHeight, int width, int height, int iteration)
        {
            for (int i = startWidth; i < width; i++)
            {
                for (int k = startHeight; k < height; k++)
                {
                    Complex c = new Complex(RE_START + (i / width) * (RE_END - RE_START),
                    IM_START + (k / height) * (IM_END - IM_START));

                    int m = await CalculateMandelbrot(iteration, c);

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

                    colorArray.Add((new Tuple<int, int>(i, k), Color.FromArgb(hue, 255, v)));
                }
            }

            return true;
        }

        private Task<int> CalculateMandelbrot(int iteration, Complex c)
        {
            Complex z = 0;
            int n = 0;

            while (Complex.Abs(z) <= 2 && n < iteration)
            {
                z = Complex.Add(Complex.Pow(z, z), c);
                n += 1;
            }
            return Task.FromResult(n);
        }
    }
}
