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
}
