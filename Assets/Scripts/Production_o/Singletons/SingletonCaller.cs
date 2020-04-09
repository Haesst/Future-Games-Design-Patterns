using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonCaller : MonoBehaviour
{

    [ContextMenu("Load JSON file")]
    public void LoadJSONFile()
    {
        string data = ResourcesManager.Instance.GetJsonData();
        Debug.Log(data);
    }

    [ContextMenu("Call POCO Singleton")]
    public void CallPOCO()
    {
        Debug.Log(FileManager.Instance.GetFileName());
    }

    [ContextMenu("Spawn Enemy")]
    public void SpawnEnemy()
    {
        EnemyManager.Instance.SpawnEnemy();
    }
}
