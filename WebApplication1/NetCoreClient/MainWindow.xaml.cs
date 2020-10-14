using System;
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
            //MandelBrotRequest mbr = new MandelBrotRequest();
            //mbr.Height = 400;
            //mbr.Width = 600;
            //mbr.Iteration = 100;
            //GetBitMapAsync(host, mbr);
        }

        
    }
}
