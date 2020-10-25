using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestGitHub.Libraries.Templates
{
    /// <summary>
    /// BitmapSourceConverter.
    /// </summary>
    public static class BitmapSourceConverter
    {
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// BitmapをBitmapSourceに変換.
        /// https://taktak.jp/2017/01/18/1753.
        /// </summary>
        /// <param name="source">Bitmap.</param>
        /// <returns>BitmapSource.</returns>
        public static BitmapSource ToBetterBitmapSource(Bitmap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var handle = source.GetHbitmap();

            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    handle,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        /// <summary>
        /// BitmapSourceをARGB形式のBitmap に変換.
        /// http://www.ruche-home.net/.
        /// </summary>
        /// <param name="src">BitmapSource. </param>
        /// <returns>Bitmap.</returns>
        public static Bitmap ToBitmap(BitmapSource src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            // フォーマットが異なるならば変換.
            var s = src;
            if (s.Format != PixelFormats.Bgra32)
            {
                s = new FormatConvertedBitmap(
                    s,
                    PixelFormats.Bgra32,
                    null,
                    0);
                s.Freeze();
            }

            // ピクセルデータをコピー
            var width = (int)s.Width;
            var height = (int)s.Height;
            var stride = width * 4;
            var datas = new byte[stride * height];
            s.CopyPixels(datas, stride, 0);

            // Bitmap へピクセルデータ書き出し
            var dst = new Bitmap(
                width,
                height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            System.Drawing.Imaging.BitmapData dstBits = null;

            try
            {
                dstBits = dst.LockBits(
                    new Rectangle(0, 0, width, height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Marshal.Copy(datas, 0, dstBits.Scan0, datas.Length);
            }
            catch
            {
                dst.Dispose();
                dst = null;
                throw;
            }
            finally
            {
                if (dst != null && dstBits != null)
                {
                    dst.UnlockBits(dstBits);
                }
            }

            return dst;
        }

        public static byte[] ToBytesBGRA32(BitmapSource src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            // フォーマットが異なるならば変換.
            var s = src;
            if (s.Format != PixelFormats.Bgra32)
            {
                s = new FormatConvertedBitmap(
                    s,
                    PixelFormats.Bgra32,
                    null,
                    0);
                s.Freeze();
            }

            // ピクセルデータをコピー
            var width = (int)s.Width;
            var height = (int)s.Height;
            var stride = width * 4;
            var datas = new byte[stride * height];
            s.CopyPixels(datas, stride, 0);

            return datas;
        }

    }
}
