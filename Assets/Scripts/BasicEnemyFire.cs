using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyFire : MonoBehaviour {

    public GameObject projectileSpawner;
    public GameObject scope;
    public GameObject target;
    public float fireRate;
    public float scopeRate;
    public float aggroDis;
    public float delay;
    internal bool firing = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (target && firing)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            if (distance < aggroDis && !IsInvoking("FireProjectile"))
            {
                if(scope)
                {
                    InvokeRepeating("AimScope",0,1/scopeRate);
                }
                InvokeRepeating("FireProjectile", delay, 1 / fireRate);
            }
            else if (distance >= aggroDis)
            {
                CancelInvoke("AimScope");
                CancelInvoke("FireProjectile");
            }
        }
        else
        {
            CancelInvoke("FireProjectile");
            CancelInvoke("AimScope");
        }
	}

    void FireProjectile()
    {
        if (firing)
        {
            CancelInvoke("AimScope");
            GameObject shotInstance = Instantiate(projectileSpawner, transform.position, transform.rotation);
            shotInstance.GetComponent<ProjectilSpawner>().destroySelf = true;
        }
    }

    void AimScope()
    {
        GameObject shotInstance = Instantiate(scope, transform.position, transform.rotation);
        shotInstance.GetComponent<ProjectilSpawner>().destroySelf = true;
    }
}
