using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

	public GameObject projectileSpawner;
	public float delay;
	float timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.fixedDeltaTime;
		if (Input.GetKey (KeyCode.Space) && timer > delay)
		{
            FireProjectile();
			timer = 0;
		}
    }

    void FireProjectile()
    {
        GameObject shotInstance = Instantiate(projectileSpawner, transform.position, transform.rotation);
        shotInstance.GetComponent<ProjectilSpawner>().destroySelf = true;
    }
}
