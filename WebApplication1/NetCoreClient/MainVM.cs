using ClientApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetworkLibraryMandelBrot;
using ProcessWatcher.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NetCoreClient
{
    public class MainVM : INotifyPropertyChanged
    {
        private bool isHeightCorrect;
        private int width;
        private bool isWidthCorrect;
        private int height;
        private int itterations;
        private bool isItterationsCorrect;
        private BitmapImage result;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainVM()
        {
            
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value <= 0)
                {
                    this.isWidthCorrect = false;
                    throw new Exception("Error width cant be smaller than zero");
                }

                this.isWidthCorrect = true;
                this.width = value;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value <= 0)
                {
                    this.isHeightCorrect = false;
                    throw new Exception("Error height cant be smaller than zero");
                }

                this.isHeightCorrect = true;
                this.height = value;
            }
        }

        public int Iterrations
        {
            get
            {
                return this.itterations;
            }
            set
            {
                if (value <= 0)
                {
                    this.isItterationsCorrect = false;
                    throw new Exception("Error itterations cant be smaller than zero");
                }

                this.isItterationsCorrect = true;
                this.itterations = value;
            }
        }

        public BitmapImage Result
        {
            get
            {
                return this.result;
            }

            set
            {
                if (value == null)
                {
                    throw new Exception("Error Image cant be null!");
                }

                this.result = value;
                this.FireOnPropertyChanged();
            }
        }

        public ICommand SendToServer
        {
            get
            {
                return new Command(async obj => {
                    if (this.height <= 0 ||this.width <= 0 || this.Iterrations <= 0)
                    {
                        MessageBox.Show("Error! Please check all values");
                        return;
                    }

                    MandelBrotRequest mbr = new MandelBrotRequest();
                    mbr.Height = this.Height;
                    mbr.Width = this.Width;
                    mbr.Iteration = this.Iterrations;
                    
                    var host = await this.CreateHost();
                    await GetBitMapAsync(host, mbr);
                    MessageBox.Show("Request successfully send to client!");
                });
            }
        }

        private async Task<IHost> CreateHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient("mandelbrot", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:51318");
                    }).AddTypedClient<MandelBrotClientService>();
                }).Build();
        }

        private async Task GetBitMapAsync(IHost host, MandelBrotRequest request)
        {

            try
            {
                var service = host.Services.GetService<MandelBrotClientService>();
                Bitmap bm = await service.GetMandelbrotBitmap(request);
                this.Result = BitmapToImageSource(bm);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        protected virtual void FireOnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
