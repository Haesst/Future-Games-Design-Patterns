using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveProperty<T> : IObservable<T>
{
    private T m_Value;
    public event Action<T> OnValueChanged;

    public void Set(T newValue)
    {
        m_Value = newValue;
        OnValueChanged.Invoke(newValue);
    }

    public T Get()
    {
        return m_Value;
    }
}
