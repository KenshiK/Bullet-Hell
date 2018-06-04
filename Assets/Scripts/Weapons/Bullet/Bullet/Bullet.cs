using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    protected Rigidbody rb;
    
    private int damages=1;

    private string _parent = "";

    public string Parent
    {
        get { return _parent; }
        set {
            //if (_parent == "")
                _parent = value;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected IEnumerator ReturnToPool(float timetoReturn)
    {
        rb.velocity = Vector3.zero;
        Parent = null;
        rb.useGravity = false;
        yield return new WaitForSeconds(timetoReturn);
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("BulletResetter"))
        {
            ReturnToPool(0);
        }
        else if ( !other.tag.Equals(Parent) && (other.tag.Equals("Player") || other.tag.Equals("Enemy")) )
        {
            //Debug.Log("Parent : " + Parent);
            //Debug.Log("Je touche : " + other.tag);

            other.GetComponent<Spaceship>().ApplyDamage(damages);
            ReturnToPool(0);
        }

    }
}
