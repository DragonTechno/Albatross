using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingScript : MonoBehaviour {

    public GameObject pooledObject;
    public int pooledAmount;
    public bool willGrow = true;
    List<GameObject> objectPool;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializePool(GameObject pO, int pA, bool wG = true)
    {
        pooledObject = pO;
        pooledAmount = pA;
        willGrow = wG;
        objectPool = new List<GameObject>();
        for(int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < objectPool.Count; i++)
        {
            if(!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        if(willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            objectPool.Add(obj);
            return obj;
        }

        return null;
    }
}
