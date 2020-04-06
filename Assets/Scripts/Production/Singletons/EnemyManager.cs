using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SingletonConfig(resourcesPath: "Prefabs/EnemyManager")]
public class EnemyManager : MonoSingleton<EnemyManager>
{
    GameObjectPool enemyPool = null;

    public void SpawnEnemy()
    {
        enemyPool.Rent(true);
    }
    public void SpawnEnemy(Vector3 location)
    {
        GameObject instance = enemyPool.Rent(true);
        instance.transform.position = location;
    }
    public void SpawnEnemy(Vector3 location, Quaternion rotation)
    {
        GameObject instance = enemyPool.Rent(true);
        instance.transform.position = location;
        instance.transform.rotation = rotation;
        //Instantiate(enemyPrefab, location, rotation);
    }
}
