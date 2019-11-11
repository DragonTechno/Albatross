using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int maxEnemies;
    public float spawnDelay;
    public GameObject enemyPrefab;
    public List<GameObject> spawnedEnemies;
    public patrolHolder assignedPatrol;
    ObjectPoolingScript pooler;

    bool delay = false;

    // Start is called before the first frame update
    void Start()
    {
        pooler = FindObjectOfType<ObjectPoolingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!delay)
        {
            if (spawnedEnemies.Count < maxEnemies)
            {
                delay = true;
                StartCoroutine("Delay");
                GameObject newEnemy = pooler.GetPooledObject(enemyPrefab);
                newEnemy.transform.position = transform.position;
                newEnemy.transform.rotation = transform.rotation;

                spawnedEnemies.Add(newEnemy);
                
                newEnemy.GetComponentInChildren<turnFollow>().pHolder = assignedPatrol;
                if (GetComponent<Collider2D>() && newEnemy.GetComponent<Collider2D>())
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), newEnemy.GetComponent<Collider2D>());
                }
                newEnemy.SetActive(true);
            }
            else
            {
                
                spawnedEnemies.RemoveAll(item => item == null || !item.activeInHierarchy);
                delay = true;
                StopCoroutine("Delay");
            }
        }
        else
        {
            StartCoroutine("Delay");
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(spawnDelay);
        delay = false;
        StopAllCoroutines();
    }
}
