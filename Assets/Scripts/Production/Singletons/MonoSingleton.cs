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
                    object[] customAttributes = typeof(T).GetCustomAttributes(typeof(SecureSingletonAttribute), false);

                    if (customAttributes.Length > 0 && customAttributes[0] is SecureSingletonAttribute attribute)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                    }
                    else
                    {
                        customAttributes = typeof(T).GetCustomAttributes(typeof(SingletonConfig), false);
                        GameObject singletonPrefab = null;

                        if (customAttributes.Length > 0 && customAttributes[0] is SingletonConfig config)
                        {
                            singletonPrefab = Resources.Load<GameObject>(config.ResourcesPath);

                            if (singletonPrefab == null)
                            {
                                throw new NullReferenceException($"Could not load {(customAttributes.Length > 0 ? config.ResourcesPath : typeof(T).Name)} in resources folder.");
                            }

                            GameObject goInstance = Instantiate(singletonPrefab);
                            instance = goInstance.GetComponent<T>();

                            if (instance == null)
                            {
                                throw new NullReferenceException($"Could not get component {typeof(T).Name}");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"xx doesn't have any of the mandatory Singleton attributes");
                        }
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
