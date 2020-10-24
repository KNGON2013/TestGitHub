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
                prefs.Add(new Pref() { Message = $"{i}" });
            }

            arrayPercentage = Enumerable
                .Range(0, _ImageWidth)
                .SelectMany(_ => new byte[] { 0, 224, 0, 255 })
                .ToArray();
        }

        private string _Header;
        public string Header
        {
            get => _Header;
            set => RaisePropertyChangedIfSet(ref _Header, value, nameof(Header));
        }

        private int _Percentage;
        public int Percentage
        {
            get => _Percentage;
            set => RaisePropertyChangedIfSet(ref _Percentage, value, nameof(Percentage));
        }

        private BitmapSource _ImageBitmapSource;
        public BitmapSource ImageBitmapSource
        {
            get => _ImageBitmapSource;
            set => RaisePropertyChangedIfSet(ref _ImageBitmapSource, value, nameof(ImageBitmapSource));
        }

        private int _ImageWidth = 50;
        public int ImageWidth
        {
            get => _ImageWidth;
            set => RaisePropertyChangedIfSet(ref _ImageWidth, value, nameof(ImageWidth));
        }

        private int _ImageHeight = 15;
        public int ImageHeight
        {
            get => _ImageHeight;
            set => RaisePropertyChangedIfSet(ref _ImageHeight, value, nameof(ImageHeight));
        }

        public IEnumerable<Pref> Prefs => prefs;

        public async Task RunAsync(int value, CancellationToken token)
        {
            Percentage = 0;

            while (Percentage < 100)
            {
                Percentage++;
                ImageBitmapSource = DrawImage();

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

            prefs[0].Message = "C";
        }

        private BitmapSource DrawImage()
        {
            var width = _ImageWidth;
            var height = _ImageHeight;

            var pixelsSize = width * height * 4;
            var pixels = new byte[pixelsSize];

            var length = 4 * width * _Percentage / 100;
            for (int y = 0; y < height; y++)
            {
                Array.Copy(arrayPercentage, 0, pixels, 4 * (y * width), length);
            }

            var stride = ((width * PixelFormats.Pbgra32.BitsPerPixel) + 7) / 8; // 一行あたりのバイトサイズ

            var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, pixels, stride);
            temp.Freeze();
            return temp;
        }
    }
}
