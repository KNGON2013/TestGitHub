using System.Windows;
using TestGitHub.Views;

namespace TestGitHub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();

            mainWindow.Show();
        }
    }
}
