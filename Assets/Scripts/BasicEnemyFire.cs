using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyFire : MonoBehaviour
{
    public GameObject scope;
    public GameObject target;
    [Tooltip("Projectiles/bursts fired per second")]
    public float fireRate;
    [Tooltip("How often the scope is refreshed")]
    public float scopeRate;
    public float aggroDis;
    public float delay;
    public bool firing = true;
    [Tooltip("Velocity of bullet when fired")]
    public float speed;
    [Tooltip("How far to seperate shots, if multiple projectiles fire at once")]
    public float spread;
    [Tooltip("Low accuracy is actually better, it's the possible range it can be about the proper point.")]
    public float accuracy = 0; 
    public float colorSpread;
    public bool multiScope;
    public bool parentToObject;
    [Tooltip("How many projectiles to fire in a shot")]
    public int count = 1;
    public GameObject bullet;
    ObjectPoolingScript pooler;

    [Header("Burst Variables")]
    public bool burst;
    public int shotsPerBurst;
    int shotsFired;

    // Use this for initialization
    void OnEnable () {
        pooler = FindObjectOfType<ObjectPoolingScript>();
        if(!target)
        {
            target = FindObjectOfType<PlaneFlight>().gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (target && firing)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            if (distance < aggroDis && !IsInvoking("Shoot"))
            {
                if(scope)
                {
                    InvokeRepeating("AimScope",0,1/scopeRate);
                }
                InvokeRepeating("Shoot", delay, 1 / fireRate);
            }
            else if (distance >= aggroDis)
            {
                CancelInvoke("AimScope");
                CancelInvoke("Shoot");
            }
        }
        else
        {
            CancelInvoke("Shoot");
            CancelInvoke("AimScope");
            shotsFired = 0;
        }
	}

    void Shoot()
    {
        if (firing)
        {
            ++shotsFired;
            FireProjectile(bullet, count);
            CancelInvoke("AimScope");
        }
        if (burst && shotsFired > shotsPerBurst)
        {
            shotsFired = 0;
            CancelInvoke("Shoot");
        }
    }

    void FireProjectile(GameObject projectile, int pCount)
    {
        for (int i = 0; i < count; ++i)
        {
            //print("FIRING!");
            GameObject newProjectile = pooler.GetPooledObject(bullet);
            newProjectile.transform.position = transform.position;
            newProjectile.transform.rotation = transform.rotation;

            int middle = (count - 1) / 2;
            Quaternion trueRotation = Quaternion.AngleAxis(transform.eulerAngles.z + spread*(middle-i)/count + Random.Range(-accuracy, accuracy), Vector3.forward);
            newProjectile.transform.right = trueRotation * Vector3.right;

            newProjectile.SetActive(true);

            if (!parentToObject)
            {
                newProjectile.GetComponent<Rigidbody2D>().velocity = trueRotation * Vector3.right * speed;
            }
            else
            {
                newProjectile.transform.parent = transform;
            }

            if (colorSpread > 0)
            {
                SpriteRenderer bulletSprite = newProjectile.GetComponent<SpriteRenderer>();
                Color bColor = bulletSprite.color;
                float h = 0;
                float s = 0;
                float v = 0;
                Color.RGBToHSV(bColor, out h, out s, out v);
                h += Mathf.Clamp(colorSpread * Mathf.Abs(Vector2.Angle(transform.right, trueRotation * Vector2.right)), 0, spread * colorSpread / 2);
                if (h > 1)
                {
                    h -= 1;
                }
                bColor = Color.HSVToRGB(h, s, v);
                bulletSprite.color = bColor;
            }
        }
    }

    void AimScope()
    {
        if (multiScope)
        {
            FireProjectile(scope, count);
        }
        else
        {
            FireProjectile(scope, 1);
        }
    }
}
