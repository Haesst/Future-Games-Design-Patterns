using System;

[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class SecureSingletonAttribute : Attribute { }

/// <summary>
/// We have to set the path where this object is loaded from resources folder,
/// if the object doesn't include this attribute an invalid operation exception
/// will be thrown.
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class SingletonConfig : Attribute 
{ 
    public string ResourcesPath { get; private set; }

    public SingletonConfig(string resourcesPath)
    {
        ResourcesPath = resourcesPath;
    }
}