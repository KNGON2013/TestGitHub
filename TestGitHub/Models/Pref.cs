using TestGitHub.Libraries.Templates;

namespace TestGitHub.Models
{
    public class Pref : ModelBase
    {
        private string _Message;
        public string Message
        {
            get => _Message;
            set => RaisePropertyChangedIfSet(ref _Message, value, nameof(Message));
        }
    }
}
