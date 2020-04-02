using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SecureSingleton]
public class ResourcesManager : MonoSingleton<ResourcesManager>
{
    public string GetJsonData()
    {
        return "JSON-data";
    }
}
