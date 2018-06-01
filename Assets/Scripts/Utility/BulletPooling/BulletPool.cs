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

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, string parentTag)
    {
        if (!poolDictionnary.ContainsKey(tag))
        {
            Debug.LogError("[BulletPool]:: " + tag + " not found");
            return null;
        }

        GameObject obj = poolDictionnary[tag].Dequeue();
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        obj.GetComponent<Bullet>().Parent = parentTag;

        poolDictionnary[tag].Enqueue(obj);

        return obj;
    }

    public int GetActiveBullets()
    {
        int bullets = 0;
        foreach(KeyValuePair<string, Queue<GameObject>> p in poolDictionnary)
        {
            foreach(GameObject go in p.Value)
            {
                if(go.activeSelf == true)
                {
                    bullets++;
                }
            }
        }
        return bullets;
    }

    void OnGUI()
    {
        int width = Screen.width, height = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, width, height * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = height * 2 / 100;
        style.normal.textColor = Color.white;
        string text = "Active bullets : " + GetActiveBullets();
        GUI.Label(rect, text, style);
    }
}

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}
