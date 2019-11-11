using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingScript : MonoBehaviour {

    [System.Serializable]
    public class Pool
    {
        public int pooledAmount;
        public bool willGrow;
        public GameObject objectType;
        public List<GameObject> objectPool;
    }

    [System.Serializable]
    public class PoolDictionary : SerializableDictionary<string, Pool>
    {
    }

    [SerializeField] PoolDictionary objectPools = new PoolDictionary();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializePool(GameObject pO, int pA, string objectName, bool wG = true)
    {
        Pool newPool = new Pool();
        newPool.objectType = pO;
        newPool.pooledAmount = pA;
        newPool.willGrow = wG;
        newPool.objectPool = new List<GameObject>();

        for(int i = 0; i < newPool.pooledAmount; i++)
        {
            GameObject obj = Instantiate(newPool.objectType);
            obj.SetActive(false);
            newPool.objectPool.Add(obj);
        }

        objectPools[objectName] = newPool;
    }

    public GameObject GetPooledObject(GameObject poolObject)
    {
        string objectName = poolObject.name;
        if(!objectPools.ContainsKey(objectName))
        {
            InitializePool(poolObject, 20, objectName);
        }

        Pool pool = objectPools[objectName];
        List<GameObject> objectPool = pool.objectPool;
        for(int i = 0; i < objectPool.Count; i++)
        {
            if(!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        if(pool.willGrow)
        {
            GameObject obj = Instantiate(pool.objectType);
            obj.SetActive(false);
            objectPool.Add(obj);
            return obj;
        }

        return null;
    }

    public bool Contains(string name)
    {
        return objectPools.ContainsKey(name);
    }
}
