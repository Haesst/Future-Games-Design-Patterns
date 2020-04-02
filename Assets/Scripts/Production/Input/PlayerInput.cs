using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            GameObject startObject = GameObject.FindGameObjectWithTag("EnemyBase");
            if (startObject != null)
            {
                Vector3 startLocation = startObject.transform.position;
                startLocation.y += 0.75f;
                EnemyManager.Instance.SpawnEnemy(startLocation);
            }
        }
    }

    private void MoveCamera()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
    }
}
