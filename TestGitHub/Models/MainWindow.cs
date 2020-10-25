using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TestGitHub.Interfaces;
using TestGitHub.Libraries.Devices;
using TestGitHub.Libraries.Devices.ComPort;
using TestGitHub.Libraries.Templates;

namespace TestGitHub.Models
{
    public class MainWindow : ModelBase, IMainWindow
    {
        private readonly string defaultTitle;

        private readonly ObservableCollection<Item> items = new ObservableCollection<Item>();

        private CancellationTokenSource cts;

        private byte bitmapCounter;

        private byte[] bitmapBGRA32;

        private byte[] tempBGRA32;

        private int bitmapWidth;

        private int bitmapHeight;

        public MainWindow()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var version = assembly.Version;

            this.defaultTitle = $"{assembly.Name} {version.Major}.{version.Minor}.{version.Build}";
            this.Title = this.defaultTitle;

            var path = Environment.CurrentDirectory + "\\Lenna.jpg";
            if (File.Exists(path))
            {
                var bitmap = new Bitmap(Image.FromFile(path));
                this.ImageBitmapSource = BitmapSourceConverter.ToBetterBitmapSource(bitmap);

                this.bitmapBGRA32 = BitmapSourceConverter.ToBytesBGRA32(this._ImageBitmapSource);
                this.bitmapWidth = bitmap.Width;
                this.bitmapHeight = bitmap.Height;
                this.tempBGRA32 = new byte[this.bitmapBGRA32.Length];

                Debug.WriteLine($"{this.bitmapBGRA32.Length} {this.bitmapWidth} {this.bitmapHeight}");

                var dispatcher = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(50),
                };

                dispatcher.Tick += ((sender, e) =>
                {
                    this.ImageBitmapSource = this.DrawLenna();
                });

                dispatcher.Start();
            }
            else
            {
                var dispatcher = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(100),
                };

                dispatcher.Tick += ((sender, e) =>
                {
                    this.ImageBitmapSource = this.DrawSample();
                });

                dispatcher.Start();
            }

        }

        private string _Title;
        public string Title
        {
            get => this._Title;
            set => this.RaisePropertyChangedIfSet(ref this._Title, value, nameof(this.Title));
        }

        private bool _EnableCommand = true;
        public bool EnableCommand
        {
            get => this._EnableCommand;
            set => this.RaisePropertyChangedIfSet(ref this._EnableCommand, value, nameof(this.EnableCommand));
        }

        public IEnumerable<Item> Items => this.items;

        private BitmapSource _ImageBitmapSource;
        public BitmapSource ImageBitmapSource
        {
            get => this._ImageBitmapSource;
            set => this.RaisePropertyChangedIfSet(ref this._ImageBitmapSource, value, nameof(this.ImageBitmapSource));
        }

        private bool _IsBoolToValueConverter;
        public bool IsBoolToValueConverter
        {
            get => this._IsBoolToValueConverter;
            set => this.RaisePropertyChangedIfSet(ref this._IsBoolToValueConverter, value, nameof(this.IsBoolToValueConverter));
        }

        private string _ViewSizeChanged;
        public string ViewSizeChanged
        {
            get => this._ViewSizeChanged;
            set => this.RaisePropertyChangedIfSet(ref this._ViewSizeChanged, value, nameof(this.ViewSizeChanged));
        }

        private int _ValidationInteger;
        public int ValidationInteger
        {
            get => this._ValidationInteger;
            set => this.RaisePropertyChangedIfSet(ref this._ValidationInteger, value, nameof(this.ValidationInteger));
        }

        private double _ValidationDouble;
        public double ValidationDouble
        {
            get => this._ValidationDouble;
            set
            {
                if (value < -100)
                {
                    throw new ArgumentException();
                }

                this.RaisePropertyChangedIfSet(ref this._ValidationDouble, value, nameof(this.ValidationDouble));
            }
        }

        private string _ViewLocationChanged;
        public string ViewLocationChanged
        {
            get => this._ViewLocationChanged;
            set => this.RaisePropertyChangedIfSet(ref this._ViewLocationChanged, value, nameof(this.ViewLocationChanged));
        }

        private ICommand _CommandTestBoolToValueConverter;
        public ICommand CommandTestBoolToValueConverter => this._CommandTestBoolToValueConverter ??= new RelayCommand(() =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            this.IsBoolToValueConverter = !this._IsBoolToValueConverter;
        });

        private ICommand _CommandAdd;
        public ICommand CommandAdd =>
            this._CommandAdd ??= new RelayCommand(() =>
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
                this.items.Add(new Item()
                {
                    Header = $"No.{this.items.Count}",
                    Percentage = 0,
                });
            });

        private ICommand _CommandRun;
        public ICommand CommandRun =>
            this._CommandRun ??= new RelayCommand(async () =>
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

                this.EnableCommand = false;

                this.cts = new CancellationTokenSource();

                var token = this.cts.Token;
                var tasks = GetTasks(token);

                IEnumerable<Task> GetTasks(CancellationToken token)
                {
                    for (int i = 0; i < this.items.Count; i++)
                    {
                        yield return this.items[i].RunAsync(i + 1, token);
                    }
                }


                await Task.WhenAll(tasks).ContinueWith(_ =>
                {
                    Debug.WriteLine("ContinueWith.");

                    this.cts.Dispose();
                    this.cts = null;

                    if (_.IsCanceled)
                    {
                        Debug.WriteLine("IsCanceled.");
                    }
                });

                Debug.WriteLine("End");
                this.EnableCommand = true;
            });

        private ICommand _CommandCancel;
        public ICommand CommandCancel =>
            this._CommandCancel ??= new RelayCommand(() =>
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

                if (this.cts != null)
                {
                    this.cts.Cancel();
                }
            });

        private ICommand _CommandDeviceChanged;
        public ICommand CommandDeviceChanged => this._CommandDeviceChanged ??= new RelayCommand(async () =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var enumerable = DeviceNotification.GetConnectedDevices()
                    .Where(_ => _.ClassGuid == RegisteredGuid.Ports)
                    .Select(_ => new PortItem(_.Caption))
                    .OrderBy(_ => _.Index);

                foreach (var a in enumerable)
                {
                    Debug.WriteLine($"{a.Index} {a.Name}");
                }
            });
        });

        private ICommand _CommandSizeChanged;
        public ICommand CommandSizeChanged => this._CommandSizeChanged ??= new RelayCommand<int>((_) =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            var x = _ & 0xffff;
            var y = (_ >> 16) & 0xffff;
            this.ViewSizeChanged = $"{x} {y}";
        });

        private ICommand _CommandLocationChanged;
        public ICommand CommandLocationChanged => this._CommandLocationChanged ??= new RelayCommand<int>((_) =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            var x = _ & 0xffff;
            var y = (_ >> 16) & 0xffff;
            this.ViewLocationChanged = $"{x} {y}";
        });

        private ICommand _CommandClosed;
        public ICommand CommandClosed => this._CommandClosed ??= new RelayCommand(() =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

            Application.Current.Shutdown();
        });

        private BitmapSource DrawSample()
        {
            var width = 640;
            var height = 480;

            var pixelsSize = width * height * 4;
            var pixels = new byte[pixelsSize];

            // バイト列に色情報を入れる
            byte value = this.bitmapCounter;
            this.bitmapCounter++;

            for (var x = 0; x < width * height * 4; x += 4)
            {
                var blue = value;
                var green = value;
                var red = value;
                byte alpha = 255;
                pixels[x] = blue;
                pixels[x + 1] = green;
                pixels[x + 2] = red;
                pixels[x + 3] = alpha;
                if (value < 255)
                {
                    value++;
                }
                else
                {
                    value = 0;
                }
            }

            var stride = ((width * PixelFormats.Pbgra32.BitsPerPixel) + 7) / 8; // 一行あたりのバイトサイズ

            var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, pixels, stride);
            temp.Freeze();

            return temp;
        }

        private BitmapSource DrawLenna()
        {
            var width = this.bitmapWidth;
            var height = this.bitmapHeight;

            var pixelsSize = this.bitmapBGRA32.Length;

            Buffer.BlockCopy(this.bitmapBGRA32, 0, this.tempBGRA32, 0, pixelsSize);

            // バイト列に色情報を入れる
            byte value = this.bitmapCounter;
            this.bitmapCounter++;

            for (var x = 0; x < width * height * 4; x += 4)
            {
                byte alpha = value;
                this.tempBGRA32[x + 3] = alpha;
            }

            var stride = ((width * PixelFormats.Pbgra32.BitsPerPixel) + 7) / 8; // 一行あたりのバイトサイズ

            var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, this.tempBGRA32, stride);
            temp.Freeze();

            return temp;
        }
    }
}
