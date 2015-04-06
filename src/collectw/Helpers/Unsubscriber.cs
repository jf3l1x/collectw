using System;
using System.Collections.Generic;

namespace CollectW.Helpers
{
    internal class Unsubscriber<T> : IDisposable
    {
        private readonly IObserver<T> _observer;
        private readonly List<IObserver<T>> _observers;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null) _observers.Remove(_observer);
        }
    }
}