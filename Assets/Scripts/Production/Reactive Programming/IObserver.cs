using System;

public interface IObserver<T> : IDisposable
{
    event Action<T> OnValueChanged;
}
