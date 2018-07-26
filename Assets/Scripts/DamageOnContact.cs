using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour {
	internal int damage;
	internal float initWidth;

	// Use this for initialization
	void Start () {
		damage = GetComponent<DamageHandler>().damage;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		GameObject other = coll.gameObject;
		print ("Explosion collision with " + other.tag);
		if (other.GetComponentInParent<Health>())
		{
			print ("Damage");
			other.GetComponentInParent<Health> ().takeDamage(damage, gameObject);
		}
        Physics2D.IgnoreCollision(coll.collider,GetComponent<Collider2D>());
	}

    internal void setDamage(int dmg)
    {
        damage = dmg;
    }
}
