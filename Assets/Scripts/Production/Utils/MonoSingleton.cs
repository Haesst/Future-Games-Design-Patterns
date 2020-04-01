using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                T[] instances = FindObjectsOfType<T>();

                if (instances.Length > 1)
                {
                    throw new InvalidOperationException("h");
                }

                if(instances.Length > 0)
                {
                    instance = instances[0];
                }

                if(instance == null)
                {
                    GameObject singletonPrefab = Resources.Load<GameObject>(typeof(T).Name);

                    if(singletonPrefab == null)
                    {
                        throw new NullReferenceException("t");
                    }

                    GameObject goInstance = Instantiate(singletonPrefab);
                    instance = goInstance.GetComponent<T>();

                    if(instance == null)
                    {
                        throw new NullReferenceException("t");
                    }
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            throw new InvalidOperationException("fwepko");
        }
    }
}
