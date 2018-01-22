using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InSituVisualization.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Fires PropertyChanged event when value of the property changes and return true. 
        /// it does not fire the event when the value has not changed and return false. 
        /// The basic idea is, that SetProperty method is virtual and you can extend it in more concrete class, 
        /// e.g to trigger validation, or by calling PropertyChanging event.
        /// </summary>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
