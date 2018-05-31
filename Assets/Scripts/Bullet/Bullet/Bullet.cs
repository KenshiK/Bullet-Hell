using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Rigidbody rb;
    private string parent = "";

    public string Parent
    {
        get { return parent; }
        set {
            if (parent == "")
                parent = value;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void ReturnToPool()
    {
        rb.velocity = Vector3.zero;
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("BulletResetter"))
        {
            ReturnToPool();
        }
        else if ( !other.tag.Equals(Parent) && (other.tag.Equals("Player") || other.tag.Equals("Enemy")) )
        {
            other.GetComponent<Spaceship>().ApplyDamage();
            ReturnToPool();
        }

    }
}
