using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TestGitHub.Libraries.Templates
{
    public abstract class ViewModelBase<TModel> : IDisposable
        where TModel : INotifyPropertyChanged
    {
        private readonly IWeakEventListener propertyChangedListener;

        private bool disposedValue = false;

        public event PropertyChangedEventHandler PropertyChanged;

        protected TModel Model { get; private set; }

        protected ViewModelBase(TModel model)
        {
            this.Model = model;

            // IWeakEventListenerを作成してEventManagerに渡す.
            // このときListenerのインスタンスは、フィールドなどで管理して
            // ViewModelがGCの対象になるまで破棄されないようにする.
            this.propertyChangedListener = new PropertyChangedWeakEventListener(
                    this.RaisePropertyChanged);

            PropertyChangedEventManager.AddListener(
                this.Model,
                this.propertyChangedListener,
                string.Empty);
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄 (マネージド オブジェクト).
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライド.
                // TODO: 大きなフィールドを null に設定.
                PropertyChangedEventManager.RemoveListener(
                    this.Model,
                    this.propertyChangedListener,
                    string.Empty);

                this.disposedValue = true;
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PropertyChangedWeakEventListener : IWeakEventListener
        {
            private readonly Action<string> raisePropertyChangedAction;

            public PropertyChangedWeakEventListener(Action<string> raisePropertyChangedAction)
            {
                this.raisePropertyChangedAction = raisePropertyChangedAction;
            }

            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                // PropertyChangedEventManager時のみ処理.
                if (typeof(PropertyChangedEventManager) != managerType)
                {
                    return false;
                }

                // PropertyChangedEventArgs時のみ処理.
                if (!(e is PropertyChangedEventArgs evt))
                {
                    return false;
                }

                // コンストラクタで渡されたコールバックを呼び出す
                this.raisePropertyChangedAction(evt.PropertyName);
                return true;
            }
        }
    }
}
