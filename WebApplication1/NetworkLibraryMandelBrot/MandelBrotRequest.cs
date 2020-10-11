using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibraryMandelBrot
{
    public class MandelBrotRequest
    {
        private int width;

        private int height;

        private int iteration;

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int Iteration { get => iteration; set => iteration = value; }
    }
}
