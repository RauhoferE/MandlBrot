﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientApp;
using Microsoft.Extensions.Hosting;
using NetworkLibraryMandelBrot;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetCoreClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient("mandelbrot", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:51318");
                    }).AddTypedClient<MandelBrotClientService>();
                }).Build();
            MandelBrotRequest mbr = new MandelBrotRequest();
            mbr.Height = 400;
            mbr.Width = 600;
            mbr.Iteration = 80;
            GetBitMapAsync(host, mbr);
        }

        public async Task GetBitMapAsync(IHost host, MandelBrotRequest request)
        {

            try
            {
                var service = host.Services.GetService<MandelBrotClientService>();
                Bitmap bm = await service.GetMandelbrotBitmap(request);
                this.image1.Source = BitmapToImageSource(bm);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}