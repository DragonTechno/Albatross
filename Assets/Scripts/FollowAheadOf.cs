using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAheadOf : MonoBehaviour
{
    public float maxDistance;
    public float followSpeed;
    public float rangeAhead;
    public GameObject target;
    Vector2 targetVector;
    bool following;
    Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            targetVector = (Vector2)target.transform.position + target.GetComponent<Rigidbody2D>().velocity * rangeAhead;
            if (((Vector2)transform.position - targetVector).magnitude < maxDistance)
            {
                following = true;
            }
            else
            {
                following = false;
            }
            if (following)
            {
                rb.velocity = (targetVector - (Vector2)transform.position).normalized * followSpeed;
            }
        }
    }
}