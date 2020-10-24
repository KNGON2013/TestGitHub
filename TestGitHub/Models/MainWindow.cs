using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public MainWindow()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var version = assembly.Version;

            defaultTitle = $"{assembly.Name} {version.Major}.{version.Minor}.{version.Build}";
            Title = defaultTitle;

            var dispatcher = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(100),
            };

            dispatcher.Tick += ((sender, e) =>
            {
                ImageBitmapSource = DrawSample();
            });

            dispatcher.Start();
        }

        private string _Title;
        public string Title
        {
            get => _Title;
            set => RaisePropertyChangedIfSet(ref _Title, value, nameof(Title));
        }

        private bool _EnableCommand = true;
        public bool EnableCommand
        {
            get => _EnableCommand;
            set => RaisePropertyChangedIfSet(ref _EnableCommand, value, nameof(EnableCommand));
        }

        public IEnumerable<Item> Items => items;

        private BitmapSource _ImageBitmapSource;
        public BitmapSource ImageBitmapSource
        {
            get => _ImageBitmapSource;
            set => RaisePropertyChangedIfSet(ref _ImageBitmapSource, value, nameof(ImageBitmapSource));
        }

        private bool _IsBoolToValueConverter;
        public bool IsBoolToValueConverter
        {
            get => _IsBoolToValueConverter;
            set => RaisePropertyChangedIfSet(ref _IsBoolToValueConverter, value, nameof(IsBoolToValueConverter));
        }

        private string _ViewSizeChanged;
        public string ViewSizeChanged
        {
            get => _ViewSizeChanged;
            set => RaisePropertyChangedIfSet(ref _ViewSizeChanged, value, nameof(ViewSizeChanged));
        }

        private int _ValidationInteger;
        public int ValidationInteger
        {
            get => _ValidationInteger;
            set => RaisePropertyChangedIfSet(ref _ValidationInteger, value, nameof(ValidationInteger));
        }

        private double _ValidationDouble;
        public double ValidationDouble
        {
            get => _ValidationDouble;
            set
            {
                if (value < -100)
                {
                    throw new ArgumentException();
                }

                RaisePropertyChangedIfSet(ref _ValidationDouble, value, nameof(ValidationDouble));
            }
        }

        private string _ViewLocationChanged;
        public string ViewLocationChanged
        {
            get => _ViewLocationChanged;
            set => RaisePropertyChangedIfSet(ref _ViewLocationChanged, value, nameof(ViewLocationChanged));
        }

        private ICommand _CommandTestBoolToValueConverter;
        public ICommand CommandTestBoolToValueConverter => _CommandTestBoolToValueConverter ??= new RelayCommand(() =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            IsBoolToValueConverter = !_IsBoolToValueConverter;
        });

        private ICommand _CommandAdd;
        public ICommand CommandAdd =>
            _CommandAdd ??= new RelayCommand(() =>
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
                items.Add(new Item()
                {
                    Header = $"No.{items.Count}",
                    Percentage = 0,
                });
            });

        private ICommand _CommandRun;
        public ICommand CommandRun =>
            _CommandRun ??= new RelayCommand(async () =>
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

                EnableCommand = false;

                cts = new CancellationTokenSource();

                var token = cts.Token;
                var tasks = GetTasks(token);

                IEnumerable<Task> GetTasks(CancellationToken token)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        yield return items[i].RunAsync(i + 1, token);
                    }
                }


                await Task.WhenAll(tasks).ContinueWith(_ =>
                {
                    Debug.WriteLine("ContinueWith.");

                    cts.Dispose();
                    cts = null;

                    if (_.IsCanceled)
                    {
                        Debug.WriteLine("IsCanceled.");
                    }
                });

                Debug.WriteLine("End");
                EnableCommand = true;
            });

        private ICommand _CommandCancel;
        public ICommand CommandCancel =>
            _CommandCancel ??= new RelayCommand(() =>
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

                if (cts != null)
                {
                    cts.Cancel();
                }
            });

        private ICommand _CommandDeviceChanged;
        public ICommand CommandDeviceChanged => _CommandDeviceChanged ??= new RelayCommand(async () =>
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
        public ICommand CommandSizeChanged => _CommandSizeChanged ??= new RelayCommand<int>((_) =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            var x = _ & 0xffff;
            var y = (_ >> 16) & 0xffff;
            ViewSizeChanged = $"{x} {y}";
        });

        private ICommand _CommandLocationChanged;
        public ICommand CommandLocationChanged => _CommandLocationChanged ??= new RelayCommand<int>((_) =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            var x = _ & 0xffff;
            var y = (_ >> 16) & 0xffff;
            ViewLocationChanged = $"{x} {y}";
        });

        private ICommand _CommandClosed;
        public ICommand CommandClosed => _CommandClosed ??= new RelayCommand(() =>
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
            byte value = bitmapCounter;
            bitmapCounter++;

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
    }
}
