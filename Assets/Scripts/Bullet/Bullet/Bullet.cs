using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void ReturnToPool()
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("BulletResetter"))
        {
            ReturnToPool();
        }
            
    }
}
