using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

	public GameObject projectileSpawner;
    public GameObject specialSpawner;
	public float projectileDelay;
    public float specialDelay;
	float projectileTimer;
    float specialTimer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		projectileTimer += Time.fixedDeltaTime;
        specialTimer += Time.fixedDeltaTime;
        if (Input.GetKey (KeyCode.Space) && projectileTimer > projectileDelay)
		{
            FireProjectile();
			projectileTimer = 0;
		}
        if (Input.GetKey(KeyCode.LeftControl) && specialTimer > specialDelay)
        {
            FireSpecial();
            specialTimer = 0;
        }
    }

    void FireProjectile()
    {
        GameObject shotInstance = Instantiate(projectileSpawner, transform.position, transform.rotation);
        shotInstance.transform.parent = transform.root;
        shotInstance.GetComponent<ProjectilSpawner>().destroySelf = true;
    }

    void FireSpecial()
    {
        GameObject shotInstance = Instantiate(specialSpawner, transform.position, transform.rotation);
        shotInstance.transform.parent = transform.root;
        shotInstance.GetComponent<ProjectilSpawner>().destroySelf = true;
    }
}
