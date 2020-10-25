using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TestGitHub.Libraries.Templates
{
    /// <summary>
    /// BoolToValueConverter.
    /// xaml使用例.
    /// <convert:BoolToStringConverter x:Key="IsPlaying" FalseValue="再生" TrueValue="ポーズ"/>
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    public class BoolToValueConverter<T> : IValueConverter
    {
        /// <inheritdoc/>
        public T FalseValue { get; set; }

        /// <inheritdoc/>
        public T TrueValue { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return this.FalseValue;
            }
            else
            {
                return (bool)value ? this.TrueValue : this.FalseValue;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && value.Equals(this.TrueValue);
        }
    }

    public class BoolToStringConverter : BoolToValueConverter<string>
    {
    }

    public class BoolToBrushConverter : BoolToValueConverter<Brush>
    {
    }

    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility>
    {
    }

    public class BoolToObjectConverter : BoolToValueConverter<object>
    {
    }

    public class BoolToNullableBoolConverter : BoolToValueConverter<bool?>
    {
    }

    public class BoolToBoolConverter : BoolToValueConverter<bool>
    {
    }
}
