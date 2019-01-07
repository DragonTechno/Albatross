using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDecay : MonoBehaviour {

	public float bulletLife;
	DamageHandler dmg;
    public string damageTag;
    public bool destroy = true;
	float timer = 0;

	// Use this for initialization
	void Awake () {
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

	void OnTriggerEnter2D(Collider2D coll)
	{
		GameObject other = coll.gameObject;
		if (other.tag == damageTag)
		{
            print("Bullet!");
            other.GetComponentInParent<Health>().takeDamage(dmg.damage, gameObject);
            if (destroy)
            {
                Destroy(gameObject);
            }
		}
	}

    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        if (other.tag == damageTag)
        {
            print("Bullet!");
            other.GetComponentInParent<Health>().takeDamage(dmg.damage, gameObject);
            if (destroy)
            {
                Destroy(gameObject);
            }
        }
    }
}