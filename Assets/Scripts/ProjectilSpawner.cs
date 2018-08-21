using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilSpawner : MonoBehaviour {

    public GameObject bullet;
    public bool destroySelf = false;

    private void Update()
    {
        if (destroySelf)
        {
            Destroy(gameObject);
        }
    }
}
