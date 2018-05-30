using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionnary;

    #region Singleton
    public static BulletPool Instance;
    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        poolDictionnary = new Dictionary<string, Queue<GameObject>>();
        foreach(Pool pool in pools)
        {
            GameObject parent = Instantiate(new GameObject(pool.tag + 's'), transform);
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for(int i = 0; i <pool.size; ++i){
                GameObject obj = Instantiate(pool.prefab, parent.transform);
                obj.transform.position = parent.transform.position;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionnary.Add(pool.tag, objectPool);
        } 
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionnary.ContainsKey(tag))
        {
            Debug.LogError("[BulletPool]:: " + tag + " not found");
            return null;
        }

        GameObject obj = poolDictionnary[tag].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        poolDictionnary[tag].Enqueue(obj);

        return obj;
    }
}

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}
