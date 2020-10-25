using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TestGitHub.Libraries.Templates;

namespace TestGitHub.Models
{
    public class Item : ModelBase
    {
        private readonly ObservableCollection<Pref> prefs = new ObservableCollection<Pref>();

        private readonly byte[] arrayPercentage;

        public Item()
        {
            for (int i = 0; i < 8; i++)
            {
                this.prefs.Add(new Pref() { Message = $"{i}" });
            }

            this.arrayPercentage = Enumerable
                .Range(0, this._ImageWidth)
                .SelectMany(_ => new byte[] { 0, 224, 0, 255 })
                .ToArray();
        }

        private string _Header;
        public string Header
        {
            get => this._Header;
            set => this.RaisePropertyChangedIfSet(ref this._Header, value, nameof(this.Header));
        }

        private int _Percentage;
        public int Percentage
        {
            get => this._Percentage;
            set => this.RaisePropertyChangedIfSet(ref this._Percentage, value, nameof(this.Percentage));
        }

        private BitmapSource _ImageBitmapSource;
        public BitmapSource ImageBitmapSource
        {
            get => this._ImageBitmapSource;
            set => this.RaisePropertyChangedIfSet(ref this._ImageBitmapSource, value, nameof(this.ImageBitmapSource));
        }

        private int _ImageWidth = 50;
        public int ImageWidth
        {
            get => this._ImageWidth;
            set => this.RaisePropertyChangedIfSet(ref this._ImageWidth, value, nameof(this.ImageWidth));
        }

        private int _ImageHeight = 15;
        public int ImageHeight
        {
            get => this._ImageHeight;
            set => this.RaisePropertyChangedIfSet(ref this._ImageHeight, value, nameof(this.ImageHeight));
        }

        public IEnumerable<Pref> Prefs => this.prefs;

        public async Task RunAsync(int value, CancellationToken token)
        {
            this.Percentage = 0;

            while (this.Percentage < 100)
            {
                this.Percentage++;
                this.ImageBitmapSource = this.DrawImage();

                for (int i = 0; i < value; i++)
                {
                    await Task.Delay(10);

                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("IsCancellationRequested.");
                        return;
                    }

                    // token.ThrowIfCancellationRequested();
                }
            }

            this.prefs[0].Message = "C";
        }

        private BitmapSource DrawImage()
        {
            var width = this._ImageWidth;
            var height = this._ImageHeight;

            var pixelsSize = width * height * 4;
            var pixels = new byte[pixelsSize];

            var length = 4 * width * this._Percentage / 100;
            for (int y = 0; y < height; y++)
            {
                Array.Copy(this.arrayPercentage, 0, pixels, 4 * (y * width), length);
            }

            var stride = ((width * PixelFormats.Pbgra32.BitsPerPixel) + 7) / 8; // 一行あたりのバイトサイズ

            var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, pixels, stride);
            temp.Freeze();
            return temp;
        }
    }
}
