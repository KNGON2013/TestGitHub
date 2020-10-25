using Microsoft.Xaml.Behaviors;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using TestGitHub.Interfaces;

namespace TestGitHub.Views.Behaviors
{
    /// <summary>
    /// Window Behavior.
    /// </summary>
    public class MainWindowBehavior : Behavior<Window>
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowBehavior"/> class.
        /// </summary>
        ~MainWindowBehavior()
        {
            this.OnDetaching();
            Debug.WriteLine($"WindowBehavior {MethodBase.GetCurrentMethod().Name}");
        }

        /// <summary>
        /// Gets or sets behavior通知(ViewToBehavior).
        /// </summary>
        public static ICommand CommandSizeChanged { get; set; }

        /// <summary>
        /// Gets or sets behavior通知(ViewToBehavior).
        /// </summary>
        public static ICommand CommandLocationChanged { get; set; }

        /// <summary>
        /// Gets or sets behavior通知(ViewToBehavior).
        /// </summary>
        public static ICommand CommandDeviceChanged { get; set; }

        /// <summary>
        /// Gets or sets behavior通知(ViewToBehavior).
        /// </summary>
        public static ICommand CommandClosed { get; set; }

        /// <summary>
        /// コマンド登録.
        /// </summary>
        protected override void OnAttached()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

            this.AssociatedObject.Loaded += (o, e) =>
            {
                var source = HwndSource.FromHwnd(new WindowInteropHelper(this.AssociatedObject).Handle);

                // var source = (HwndSource)HwndSource.FromVisual(this.AssociatedObject); // 汎用変換.
                source.AddHook(new HwndSourceHook(WndProc));

                Debug.WriteLine("WindowBehavior OnAttached");
                if (this.AssociatedObject.DataContext != null)
                {
                    CommandDeviceChanged ??= ((IMainWindowBehavior)this.AssociatedObject.DataContext).CommandDeviceChanged;
                    CommandSizeChanged ??= ((IMainWindowBehavior)this.AssociatedObject.DataContext).CommandSizeChanged;
                    CommandLocationChanged ??= ((IMainWindowBehavior)this.AssociatedObject.DataContext).CommandLocationChanged;
                    CommandClosed ??= ((IMainWindowBehavior)this.AssociatedObject.DataContext).CommandClosed;
                }
            };
        }

        /// <summary>
        /// コマンド登録解除.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        /// <summary>
        /// ウィンドウプロシージャ.
        /// </summary>
        /// <param name="hwnd">IntPtr : ウィンドウハンドル.</param>
        /// <param name="msg">int : メッセージ..</param>
        /// <param name="wParam">wIntPtr : パラメータ.</param>
        /// <param name="lParam">lIntPtr : パラメータ.</param>
        /// <param name="handled">ref book : ハンドルフラグ.</param>
        /// <returns>IntPtr.</returns>
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WindowsMessage)msg)
            {
                case WindowsMessage.WM_CLOSE:
                    CommandClosed.Execute(wParam.ToInt32());
                    break;
                case WindowsMessage.WM_DEVICECHANGE:
                    // Debug.WriteLine((DeviceNotification.DBT)wParam.ToInt32());
                    if ((long)wParam < 0x80000000)
                    {
                        CommandDeviceChanged.Execute(wParam.ToInt32());
                    }

                    break;
                case WindowsMessage.WM_SIZE:
                    if ((long)lParam < 0x80000000)
                    {
                        CommandSizeChanged.Execute(lParam.ToInt32());
                    }

                    break;
                case WindowsMessage.WM_MOVE:
                    if ((long)lParam < 0x80000000)
                    {
                        CommandLocationChanged.Execute(lParam.ToInt32());
                    }

                    break;
            }

            return IntPtr.Zero;
        }
    }
}
