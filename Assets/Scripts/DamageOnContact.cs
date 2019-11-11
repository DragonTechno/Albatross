using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour {
    public bool oneHit;
	internal int damage;
	internal float initWidth;

	// Use this for initialization
	void Start () {
		damage = GetComponent<DamageHandler>().damage;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
        GameObject other = coll.gameObject;
        if (other.GetComponentInParent<Health>())
        {
            other.GetComponentInParent<Health>().takeDamage(damage, gameObject);
        }
        if (oneHit)
        {
            Physics2D.IgnoreCollision(coll, GetComponent<Collider2D>());
        }
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject other = coll.gameObject;
        if (other.GetComponentInParent<Health>())
        {
            other.GetComponentInParent<Health>().takeDamage(damage, gameObject);
        }
        if (oneHit)
        {
            Physics2D.IgnoreCollision(coll.collider, GetComponent<Collider2D>());
        }
    }

    internal void setDamage(int dmg)
    {
        damage = dmg;
    }
}
