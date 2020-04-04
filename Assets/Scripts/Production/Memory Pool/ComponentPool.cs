using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentPool<T> : IDisposable, IPool<T> where T : Component
{
    private bool m_Disposed;
    private readonly uint m_ExpandBy;
    
    private readonly Stack<T> m_Ojbects = new Stack<T>();
    private readonly List<T> m_Created = new List<T>();

    private readonly T m_Prefab;
    private readonly Transform m_Parent;

    public ComponentPool(uint initSize, T prefab, uint expandBy = 1, Transform parent = null)
    {
        m_ExpandBy = expandBy < 1 ? 1 : expandBy;
        m_Prefab = prefab;
        m_Parent = parent;

        Expand((uint)Mathf.Max(1, initSize));
    }

    private void Expand(uint amount)
    {
        for(int i = 0; i < amount; i++)
        {
            T instance = UnityEngine.Object.Instantiate(m_Prefab, m_Parent);
            instance.gameObject.AddComponent<EmitOnDisable>().OnDisableGameObject += Unrent;
            m_Ojbects.Push(instance);
            m_Created.Add(instance);
        }
    }

    public T Rent(bool returnActive)
    {
        if(m_Ojbects.Count <= 0)
        {
            Expand(m_ExpandBy);
        }

        T instance = m_Ojbects.Pop();
        return instance;
    }

    private void Unrent(GameObject gameObject)
    {
        if(!m_Disposed)
        {
            m_Ojbects.Push(gameObject.GetComponent<T>());
        }
    }

    public void Clean()
    {
        foreach (T component in m_Created)
        {
            if (component != null)
            {
                component.GetComponent<EmitOnDisable>().OnDisableGameObject -= Unrent;
            }
        }

        m_Created.Clear();
        m_Ojbects.Clear();
    }

    public void Dispose()
    {
        m_Disposed = true;
        Clean();
    }
}
