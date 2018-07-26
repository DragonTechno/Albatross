using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicExplosion : MonoBehaviour {
	
	public float delay;
	public float radius;
	DamageHandler dmg;
	public GameObject explosion;
	internal float timer = 0;

	// Use this for initialization
	void Start () {
		dmg = GetComponent<DamageHandler> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (timer > delay)
		{
			Explode ();	
		}
		timer += Time.fixedDeltaTime;
	}

	void Explode()
	{
		if (explosion && GetComponent<SpriteRenderer>().color.a != 0)
		{
			GameObject blast = Instantiate(explosion,transform.position,Quaternion.identity);
			blast.GetComponent<DamageOnContact> ().setDamage (dmg.damage);
		}
		Destroy (gameObject);
	}
}
