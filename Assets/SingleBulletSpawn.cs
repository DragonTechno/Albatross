﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBulletSpawn : ProjectilSpawner {

    public float speed;
    public float spread;
    public float colorSpread;
    public int count = 1;

    // Use this for initialization
    void Start () {
        if (destroySelf)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                Quaternion randomRotation = Quaternion.AngleAxis(transform.eulerAngles.z + Random.Range(-spread, spread), Vector3.forward);
                newBullet.transform.right = randomRotation * Vector3.right;
                newBullet.GetComponent<Rigidbody2D>().velocity = randomRotation * Vector3.right * speed;
                if (colorSpread > 0)
                {
                    SpriteRenderer bulletSprite = newBullet.GetComponent<SpriteRenderer>();
                    Color bColor = bulletSprite.color;
                    float h = 0;
                    float s = 0;
                    float v = 0;
                    Color.RGBToHSV(bColor, out h, out s, out v);
                    h += Mathf.Clamp(colorSpread*Mathf.Abs(Vector2.Angle(transform.right,randomRotation*Vector2.right)),0,spread*colorSpread/2);
                    if(h>1)
                    {
                        h -= 1;
                    }
                    bColor = Color.HSVToRGB(h, s, v);
                    bulletSprite.color = bColor;
                }
            }
        }
	}
}
