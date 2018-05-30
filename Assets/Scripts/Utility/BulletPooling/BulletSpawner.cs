using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {

    private BulletPool bulletPooler;

    void Start()
    {
        bulletPooler = BulletPool.Instance;
    }
    private void FixedUpdate()
    {
        GameObject obj = bulletPooler.SpawnFromPool("Bullet", transform.position, Quaternion.identity);
        float xForce = Random.Range(-.1f, .1f);
        float yForce = Random.Range(0.5f, 1f);
        float zForce = Random.Range(-.1f, .1f);

        Vector3 force = new Vector3(xForce, yForce, zForce);

        obj.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
    }
}
