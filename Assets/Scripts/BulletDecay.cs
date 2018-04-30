using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDecay : MonoBehaviour {

	public float bulletLife;
	DamageHandler dmg;
	float timer = 0;

	// Use this for initialization
	void Start () {
		dmg = GetComponent<DamageHandler> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.fixedDeltaTime;
		if (timer > bulletLife)
		{
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		GameObject other = coll.gameObject;
		if (other.tag == "Damageable")
		{
			print ("Bullet!"); 
			other.GetComponent<Health> ().currentHealth -= dmg.damage;
			Destroy (gameObject);
		}
	}
}