using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour {

    [SerializeField] private string missileTag;
    [SerializeField] private int maxTargets = 10;
    [SerializeField] private float fireRate = 2.5f;
    [SerializeField] private float timesBetweenMissiles;
    [SerializeField] private int missilesPerTarget = 1;
    
    private float timer = 0;
    private BulletPool bulletPool;
    private AIManager aiManager;
    private List<GameObject> enemies;
    private void Start()
    {
        bulletPool = BulletPool.Instance;
        aiManager = AIManager.Instance;
    }
    private void Update()
    {
        if(Time.time - timer > fireRate)
        {
            enemies = aiManager.GetCurrentEnemies();
            int targetNumber = enemies.Count > maxTargets ? maxTargets : enemies.Count;
            StartCoroutine(SpawnMissiles(targetNumber));
            timer = Time.time;
        }
    }

    private IEnumerator SpawnMissiles(int toSpawn)
    {
        int count = 0;
        WaitForSeconds waitTime = new WaitForSeconds(timesBetweenMissiles);
        while(count < toSpawn)
        {
            Vehicle target = enemies[count].GetComponentInChildren<Vehicle>();
            if(target != null)
            {
                for(int i = 0; i < missilesPerTarget; i++)
                {
                    GameObject obj = bulletPool.SpawnFromPool(missileTag, transform.position, Quaternion.identity, "Player");
                    Missile mis = obj.GetComponentInChildren<Missile>();
                    IPooledObject iPooled = mis.GetComponent<IPooledObject>();
                    if (iPooled != null)
                    {
                        iPooled.OnSpawn();
                    }
                    mis.Target = target;
                }
                count++;
            }
            yield return waitTime;
        }
    }
}
