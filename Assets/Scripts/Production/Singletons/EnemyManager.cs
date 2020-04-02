using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SingletonConfig(resourcesPath: "Prefabs/EnemyManager")]
public class EnemyManager : MonoSingleton<EnemyManager>
{
    [SerializeField] private GameObject enemyPrefab = default;

    public void SpawnEnemy()
    {
        Instantiate(enemyPrefab);
    }
    public void SpawnEnemy(Vector3 location)
    {
        Instantiate(enemyPrefab, location, Quaternion.identity);
    }
    public void SpawnEnemy(Vector3 location, Quaternion rotation)
    {
        Instantiate(enemyPrefab, location, rotation);
    }
}
