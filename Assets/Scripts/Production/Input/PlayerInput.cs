using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private int spawnedEnemies = 0;
    private int targetEnemies = 0;

    GameObject enemyBase = null;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            targetEnemies += 100;
        }

        if(targetEnemies > spawnedEnemies && Time.frameCount % 27 == 0)
        {
            if(enemyBase == null)
            {
                enemyBase = GameObject.FindGameObjectWithTag("EnemyBase");
            }

            if(enemyBase != null)
            {
                Vector3 startLocation = enemyBase.transform.position;
                startLocation.y += 0.75f;

                EnemyManager.Instance.SpawnEnemy(startLocation);
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
