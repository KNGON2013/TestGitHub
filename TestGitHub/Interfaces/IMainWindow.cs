using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TestGitHub.Models;

namespace TestGitHub.Interfaces
{
    public interface IMainWindow : INotifyPropertyChanged, IMainWindowBehavior
    {
        string Title { get; }

        bool EnableCommand { get; }

        IEnumerable<Item> Items { get; }

        BitmapSource ImageBitmapSource { get; }

        bool IsBoolToValueConverter { get; set; }

        string ViewSizeChanged { get; }

        string ViewLocationChanged { get; }

        int ValidationInteger { get; set; }

        double ValidationDouble { get; set; }

        ICommand CommandTestBoolToValueConverter { get; }

        ICommand CommandAdd { get; }

        ICommand CommandRun { get; }

        ICommand CommandCancel { get; }
    }
}
