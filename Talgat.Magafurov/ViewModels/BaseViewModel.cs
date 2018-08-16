using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Talgat.Magafurov.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Предоставляет службы для управления очередью рабочих элементов потока, 
        /// которому принадлежит базовый дескриптор окна элемента управления
        /// </summary>
        protected readonly Dispatcher dispatcher;

        public BaseViewModel()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Обертка, позволяющая выполнять передаваемый метод в другом потоке
        /// </summary>
        protected Task ActionTask(Action action)
        {
            return Task.Factory.StartNew(_ => action(), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Обертка, позволяющая выполнять передаваемую функцию в другом потоке
        /// </summary>
        protected Task<T> FuncTask<T>(Func<T> func)
        {
            return Task.Factory.StartNew(_ => func(), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Обертка, позволяющая выполнять передаваемый метод в потоке, 
        /// которому принадлежит базовый дескриптор окна элемента управления
        /// </summary>
        protected void DispatcherInvoke(Action action)
        {
            dispatcher.Invoke(action);
        }
    }
}