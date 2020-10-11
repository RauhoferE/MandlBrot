using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetworkLibraryMandelBrot;
using System.Drawing;

namespace WebApplication1.Services
{
    public interface IMandelbrotService
    {
        Task<Bitmap> GetMandelBrotBitmap(MandelBrotRequest request);
    }
}
