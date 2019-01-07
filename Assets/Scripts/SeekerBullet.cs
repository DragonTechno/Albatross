using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerBullet : MonoBehaviour
{
    public float seekingSpeed;
    public float seekingRange;
    public float seekingAngle;
    public LayerMask seekingLayer;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            if(Vector2.Angle(target.transform.position - transform.position, GetComponent<Rigidbody2D>().velocity) >= seekingAngle)
            {
                target = null;
            }
        }
        if (!target)
        {
            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position,seekingRange,seekingLayer);
            float minDistance = seekingRange;
            foreach (Collider2D collider in nearby)
            {
                if (collider)
                {
                    print("Collider check");
                    Vector2 distance = collider.transform.position - transform.position;
                    if(Vector2.Angle(distance,GetComponent<Rigidbody2D>().velocity)<seekingAngle && distance.magnitude < minDistance)
                    {
                        target = collider.gameObject;
                        minDistance = distance.magnitude;
                    }
                }
            }
        }
        if(target)
        {
            print(target.name);
            Vector2 distance = target.transform.position - transform.position;

            // right keeps track of where the pointer is pointing to, or where its right side points to
            transform.right = (Vector2)Vector3.RotateTowards(transform.right, distance, seekingSpeed, 1);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            // the math part of this line makes sure that enemies goes faster when they're further away and is capped
            rb.velocity = transform.right * rb.velocity.magnitude;
        }
    }
}
