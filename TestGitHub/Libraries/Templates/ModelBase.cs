using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestGitHub.Libraries.Templates
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void PropertyChangedExec(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        protected bool RaisePropertyChangedIfSet<TResult>(
            ref TResult source,
            TResult value,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TResult>.Default.Equals(source, value))
            {
                return false;
            }

            source = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
