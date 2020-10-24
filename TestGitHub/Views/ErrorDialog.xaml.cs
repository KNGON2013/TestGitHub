using System.Windows;

namespace TestGitHub.Views
{
    /// <summary>
    /// ErrorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            buttonOK.Focus();
        }
    }
}
