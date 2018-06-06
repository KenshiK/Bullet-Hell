using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour {

    [SerializeField] private GameObject shaterred;
    [SerializeField] private float force = 100.0f;
    [SerializeField] private float radius = 100.0f;
    [SerializeField] private float upwardModifier = 25.0f;
    public ForceMode forceMode;
    private Collider[] cols;

    private void Start()
    {
        cols = new Collider[15];
    }
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject obj = Instantiate(shaterred, transform.position, Quaternion.identity, transform);
            obj.transform.parent = null;
            Physics.OverlapSphereNonAlloc(transform.position, radius, cols, LayerMask.GetMask("Planet"));
            foreach(Collider col in cols)
            {
                if(col != null)
                    col.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, upwardModifier, forceMode);
            }
            Destroy(gameObject);
        }
	}
}
