﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : IDisposable, IPool<GameObject>
{
    bool m_Disposed = false;
    readonly uint m_ExpandBy;
    readonly GameObject m_Prefab;
    readonly Transform m_Parent;

    readonly Stack<GameObject> m_Objects = new Stack<GameObject>();
    readonly List<GameObject> m_Created = new List<GameObject>();

    public GameObjectPool(uint initSize, GameObject prefab, uint expandBy = 1, Transform parent = null)
    {
        m_ExpandBy = expandBy < 1 ? 1 : expandBy; // (uint)Mathf.Max(1, expandBy); shorthand if must be quicker?
        m_Prefab = prefab;
        m_Parent = parent;

        m_Prefab.SetActive(false);
        Expand((uint)Mathf.Max(1, initSize));
    }

    private void Expand(uint amount)
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject instance = GameObject.Instantiate(m_Prefab, m_Parent);
            EmitOnDisable emitOnDisable = instance.AddComponent<EmitOnDisable>();
            emitOnDisable.OnDisableGameObject += UnRent;
            m_Objects.Push(instance);
            m_Created.Add(instance);
        }
    }

    private void UnRent(GameObject gameObject)
    {
        if (!m_Disposed)
        {
            m_Objects.Push(gameObject);
        }
    }

    public GameObject Rent(bool returnActive)
    {
        if(m_Objects.Count <= 0)
        {
            Expand(m_ExpandBy);
        }

        GameObject instance = m_Objects.Pop();

        instance.SetActive(returnActive);
        return instance;
    }

    public void Clean()
    {
        foreach (var component in m_Created)
        {
            if (component != null)
            {
                component.GetComponent<EmitOnDisable>().OnDisableGameObject -= UnRent;
            }
        }

        m_Created.Clear();
        m_Objects.Clear();
    }

    public void Dispose()
    {
        m_Disposed = true;
        Clean();
    }
}