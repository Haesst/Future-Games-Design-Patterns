using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private int spawnedEnemies = 0;

    GameObject enemyBase = null;
    [SerializeField] private GameObjectScriptablePool debugEnemyPool = default;
    MapData mapData;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            if (enemyBase == null)
            {
                enemyBase = GameObject.FindGameObjectWithTag("EnemyBase");
            }
            if (mapData == null)
            {
                mapData = GameObject.Find("Map")?.GetComponent<MapGenerator>().GetMapData();
            }

            if (mapData != null && enemyBase != null)
            {
                Vector3 startLocation = enemyBase.transform.position;
                startLocation.y += 0.75f;

                GameObject instance = debugEnemyPool.Rent(true);
                instance.transform.position = startLocation;
                instance.GetComponent<Boxymon>().Init(BoxymonType.SmallBoxymon, mapData);

                spawnedEnemies++;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (enemyBase == null)
            {
                enemyBase = GameObject.FindGameObjectWithTag("EnemyBase");
            }
            if (mapData == null)
            {
                mapData = GameObject.Find("Map")?.GetComponent<MapGenerator>().GetMapData();
            }

            if (mapData != null && enemyBase != null)
            {
                Vector3 startLocation = enemyBase.transform.position;
                startLocation.y += 0.75f;

                GameObject instance = debugEnemyPool.Rent(true);
                instance.transform.position = startLocation;
                instance.GetComponent<Boxymon>().Init(BoxymonType.BigBoxymon, mapData);

                spawnedEnemies++;
            }
        }
    }

    private void MoveCamera()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
    }
}
