using System.Windows.Input;

namespace TestGitHub.Interfaces
{
    public interface IMainWindowBehavior
    {
        ICommand CommandDeviceChanged { get; }

        ICommand CommandSizeChanged { get; }

        ICommand CommandLocationChanged { get; }

        ICommand CommandClosed { get; }
    }
}
