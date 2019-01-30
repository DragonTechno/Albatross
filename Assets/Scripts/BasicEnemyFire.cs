using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyFire : MonoBehaviour {

    public GameObject scope;
    public GameObject target;
    public float fireRate;
    public float scopeRate;
    public float aggroDis;
    public float delay;
    public bool firing = true;
    public float speed;
    public float spread;
    public float accuracy = 0; ///Low "accuracy" is actually better, it's the possible range it can be about the proper point.
    public float colorSpread;
    public bool multiScope;
    public bool parentToObject;
    public int count = 1;
    public GameObject bullet;

    [Header("Burst Variables")]
    public bool burst;
    public int shotsPerBurst;
    int shotsFired;

    // Use this for initialization
    void Start () {

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
                foreach(Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
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
            GameObject newProjectile = Instantiate(projectile, transform.position, transform.rotation);
            int middle = (count - 1) / 2;
            Quaternion trueRotation = Quaternion.AngleAxis(transform.eulerAngles.z + spread*(middle-i)/count + Random.Range(-accuracy, accuracy), Vector3.forward);
            newProjectile.transform.right = trueRotation * Vector3.right;
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
