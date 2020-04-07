using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody m_RigidBody;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    public void Init(float initForce, Vector3 direction)
    {
        m_RigidBody.velocity = direction * initForce;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Git Boxymon!");
            Destroy(gameObject);
        }
        else if(other.CompareTag("Tower"))
        {
            return;
        }
        else
        {
            Debug.Log(other.tag);
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
