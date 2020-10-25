using TestGitHub.Libraries.Templates;

namespace TestGitHub.Models
{
    public class Pref : ModelBase
    {
        private string _Message;
        public string Message
        {
            get => this._Message;
            set => this.RaisePropertyChangedIfSet(ref this._Message, value, nameof(this.Message));
        }
    }
}
