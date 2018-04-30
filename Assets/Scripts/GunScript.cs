using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

	public GameObject projectile;
	public float delay;
	public float speed;
	float timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.fixedDeltaTime;
		if (Input.GetKey (KeyCode.Space) && timer > delay)
		{
			GameObject shot = Instantiate (projectile, transform.position, Quaternion.identity);
			shot.GetComponent<Rigidbody2D> ().velocity = transform.right * speed;
			timer = 0;
		}
	}
}
