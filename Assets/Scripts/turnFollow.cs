using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnFollow : MonoBehaviour {

	public float turnSpeed; //Maximum angular velocity of the ship
	public float speed; //Maximum forward velocity of the ship
	public float followDistance; //Fight distance of the ship
    public float followHeight;
    public float riseSpeed;
    public float wayPointDis;
    public Vector2 offset;
	public string enemyType; //Type of the ship (currently unused)
    public bool moveParent = false;
	public GameObject player; //The object it follows
    public patrolHolder pHolder;
    public GameObject target;
    Transform[] patrol;
    int patrolIndex;
    Collider2D playerCollider;
    bool fighting;
    bool rising;
	PlaneManagement planeMan; //The plane data
	Rigidbody2D rb; //This object's rigid body

	// Use this for initialization
	void Start () {
        patrol = pHolder.patrol;
        target = patrol[0].gameObject;
        patrolIndex = 0;
		rb = GetComponent<Rigidbody2D> ();
		planeMan = FindObjectOfType<PlaneManagement> ();
        playerCollider = player.GetComponent<Collider2D>();
        fighting = false;
        // for testing purposes, makes sure it's keeping track of the right plane manager
        Invoke("CheckAgain", .05f);
        if (moveParent)
        {
            foreach (Collider2D child in transform.parent.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(child, playerCollider, true);
            }
        }
        else
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (target)
        {
            float pushRadius = 10f;
            float pushForce = 5f;
            Vector2 distance = Vector2.zero;
            float heightDiff = 1000;
            if (player)
            {
                distance = (Vector2)player.transform.position + (Vector2)(Quaternion.AngleAxis(player.transform.rotation.eulerAngles.z, Vector3.forward) * offset) - (Vector2)transform.position;
                heightDiff = Mathf.Abs(player.transform.position.z - transform.position.z);
            }
            if (distance.magnitude < followDistance && heightDiff < followHeight)
            {
                print("Player hunt");
                target = player;
                pushForce = pushForce * 2;
                if (!IsInvoking("FollowHeight") && !rising)
                {
                    Invoke("FollowHeight", 0);
                }
                if (!fighting)
                {
                    if (moveParent)
                    {
                        foreach (Collider2D child in transform.parent.GetComponentsInChildren<Collider2D>())
                        {
                            print(child.gameObject.name);
                            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, false);
                        }
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, false);
                    }
                    planeMan.RequestFight();
                    fighting = true;
                }
            }
            else
            {
                print("Patrolling");
                target = patrol[patrolIndex].gameObject;
                print(target);
                if (fighting)
                {
                    if (moveParent)
                    {
                        foreach (Collider2D child in transform.parent.GetComponentsInChildren<Collider2D>())
                        {
                            print(child.gameObject.name);
                            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
                        }
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
                    }
                    planeMan.LeaveFight();
                    fighting = false;
                }
                CancelInvoke("FollowHeight");
            }

            if (target != player)
            {
                distance = (Vector2)target.transform.position - (Vector2)transform.position;
                heightDiff = Mathf.Abs(target.transform.position.z - transform.position.z);
                if(distance.magnitude < wayPointDis)
                {
                    patrolIndex = (patrolIndex + 1) % patrol.Length;
                }
            }
            // right keeps track of where the pointer is pointing to, or where its right side points to
            transform.right = (Vector2)Vector3.RotateTowards(transform.right, distance, turnSpeed * Random.Range(.7f, 1.4f), turnSpeed);
            // the math part of this line makes sure that enemies goes faster when they're further away and is capped
            rb.velocity = transform.right * speed * Mathf.Clamp(Mathf.Sqrt(distance.magnitude / 10), 1, 1.5f);
            //rb.velocity = (Vector2.Dot(rb.velocity.normalized * Mathf.Clamp(rb.velocity.magnitude,speed,speed*1.5f), transform.right))*transform.right;

            // an empty list that we'll add to if there are any overlapping enemies
            Collider2D[] nearby = new Collider2D[10];
            // filter states that we're only looking for objects with the enemy layer
            ContactFilter2D enemyFilter = new ContactFilter2D();
            enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            Physics2D.OverlapCircle(transform.position, pushRadius, enemyFilter, nearby);
            foreach (Collider2D collider in nearby)
            {
                if (collider)
                {
                    if (collider.gameObject.GetComponent<turnFollow>())
                    {
                        turnFollow unit = collider.attachedRigidbody.gameObject.GetComponent<turnFollow>();
                        // calculate direction from target to me
                        if (enemyType == unit.enemyType)
                        {
                            Vector2 bvec = transform.position - unit.gameObject.transform.position;
                            bvec = bvec.normalized * Mathf.Clamp(pushForce / bvec.magnitude, 1, 100);
                            rb.AddForce(bvec);
                        }
                    }
                }
            }
        }
	}

	void FollowHeight()
	{
        rising = true;
        if (transform.position.z < target.transform.position.z)
        {
            StartCoroutine(ChangeStrata(1));
        }
        else
        {
            StartCoroutine(ChangeStrata(-1));
        }
	}

    public IEnumerator ChangeStrata(int direction)
    {
        if (target)
        {
            float targetHeight = target.transform.position.z;
            float wiggle = .1f;
            if (direction == 1)
            {
                while (transform.position.z < targetHeight - wiggle)
                {
                    if (moveParent)
                    {
                        transform.parent.position += Vector3.forward * riseSpeed;
                    }
                    else
                    {
                        transform.position += Vector3.forward * riseSpeed;
                    }
                    yield return new WaitForSecondsRealtime(.02f);
                }
            }
            else
            {
                while (transform.position.z > targetHeight + wiggle)
                {
                    if (moveParent)
                    {
                        transform.parent.position += Vector3.back * riseSpeed;
                    }
                    else
                    {
                        transform.position += Vector3.back * riseSpeed;
                    }
                    yield return new WaitForSecondsRealtime(.02f);
                }
            }
            if (moveParent)
            {
                transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, targetHeight);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, targetHeight);
            }
        }
        rising = false;
    }

    void CheckAgain()
    {
        planeMan = FindObjectOfType<PlaneManagement>();
    }

    private void OnDestroy()
    {
        if(fighting)
        {
            planeMan.LeaveFight();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.collider.gameObject.name);
        print(Physics2D.GetIgnoreCollision(collision.collider,GetComponent<Collider2D>()));
    }
}
