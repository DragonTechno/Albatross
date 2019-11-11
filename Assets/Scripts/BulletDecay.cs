using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDecay : MonoBehaviour {

	public float bulletLife;
	DamageHandler dmg;
    public string damageTag;
    public bool destroy = true;
	float timer = 0;

    private void OnEnable()
    {
        dmg = GetComponent<DamageHandler>();
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate () {
		timer += Time.fixedDeltaTime;
		if (timer > bulletLife)
		{
            gameObject.SetActive(false);
        }
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		GameObject other = coll.gameObject;
		if (other.tag == damageTag)
		{
            other.GetComponentInParent<Health>().takeDamage(dmg.damage, gameObject);
            if (destroy)
            {
                gameObject.SetActive(false);
            }
		}
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject other = coll.gameObject;
        if (other.tag == damageTag)
        {
            other.GetComponentInParent<Health>().takeDamage(dmg.damage, gameObject);
            if (destroy)
            {
                gameObject.SetActive(false);
            }
        }
    }
}