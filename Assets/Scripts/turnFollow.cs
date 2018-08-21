using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class turnFollow : MonoBehaviour {

	public float turnSpeed; //Maximum angular velocity of the ship
    public float orbitRate=1;
	public float speed; //Maximum forward velocity of the ship
	public float followDistance; //Fight distance of the ship
    public float followHeight;
    public float riseSpeed;
    public float wayPointDis;
    public Vector2 offset;
	public string enemyType; //Type of the ship (currently unused)
    public bool moveParent = false;
    public bool turn = true;
	public GameObject player; //The object it follows
    public patrolHolder pHolder;
    public GameObject target;

    [Header("Hopping Variables")]
    public bool hop = false;
    public float hopWaitTime;
    public float hopDuration;
    public int shootState = 0;
    public float shotCutoff;

    float hopDelay;
    Transform[] patrol;
    int patrolIndex;
    Collider2D playerCollider;
    bool fighting;
    bool rising;
    bool moving;
    PlaneManagement planeMan; //The plane data
	Rigidbody2D rb; //This object's rigid body
    Vector2 originalOffset;
    int slitherDir;

	// Use this for initialization
	void Start () {
        hopDelay = 0;
        slitherDir = 1;
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
        originalOffset = offset;
        moving = false;
        OffsetBehavior(Vector2.zero, true);
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (target)
        {
            float pushRadius = 10f;
            float pushForce = 5f;
            Vector2 distance = Vector2.zero;
            float heightDiff = 1000;
            if(hop)
            {
                hopDelay += Time.fixedDeltaTime;
            }
            if (player)
            {
                distance = (Vector2)player.transform.position + (Vector2)(Quaternion.AngleAxis(player.transform.rotation.eulerAngles.z, Vector3.forward) * offset) - (Vector2)transform.position;
                heightDiff = Mathf.Abs(player.transform.position.z - transform.position.z);
            }
            if (distance.magnitude < followDistance && heightDiff < followHeight)
            {
                target = player;
                pushForce = pushForce * 2;
                distance = OffsetBehavior(distance);
                if (!fighting)
                {
                    if (moveParent)
                    {
                        foreach (Collider2D child in transform.parent.GetComponentsInChildren<Collider2D>())
                        {
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
                if(hop)
                {
                    moving = false;
                    hopDelay = 0;
                }
                target = patrol[patrolIndex].gameObject;
                if (fighting)
                {
                    if (moveParent)
                    {
                        foreach (Collider2D child in transform.parent.GetComponentsInChildren<Collider2D>())
                        {
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
            if(heightDiff > .1)
            {
                if (!IsInvoking("FollowHeight") && !rising)
                {
                    Invoke("FollowHeight", 0);
                }
            }
            if (turn)
            {
                if (!hop || moving)
                {
                    // right keeps track of where the pointer is pointing to, or where its right side points to
                    transform.right = (Vector2)Vector3.RotateTowards(transform.right, distance, turnSpeed * Random.Range(.7f, 1.4f), turnSpeed);
                    // the math part of this line makes sure that enemies goes faster when they're further away and is capped
                    rb.velocity = transform.right * speed * Mathf.Clamp(Mathf.Sqrt(distance.magnitude / 5), 0, 1.5f);
                    //rb.velocity = (Vector2.Dot(rb.velocity.normalized * Mathf.Clamp(rb.velocity.magnitude,speed,speed*1.5f), transform.right))*transform.right;
                }
            }
            else
            {
                rb.velocity = distance.normalized * speed * Mathf.Clamp(Mathf.Sqrt(distance.magnitude / 5), .5f, 1.5f);
            }
            if(hop)
            {
                float wiggle = 1;
                if(moving && hopDelay > hopDuration)
                {
                    print("Timed out");
                    moving = false;
                    hopDelay = 0;
                }
                else if(!moving && hopDelay > hopWaitTime)
                {
                    //This could maybe be done better, saving the enemy firing script.
                    moving = true;
                    hopDelay = 0;
                    GetComponent<BasicEnemyFire>().firing = true;
                }
                if(moving && distance.magnitude < wiggle)
                {
                    moving = false;
                    hopDelay = 0;
                }
            }
            // an empty list that we'll add to if there are any overlapping enemies
            Collider2D[] nearby = new Collider2D[10];
            // filter states that we're only looking for objects with the enemy layer
            ContactFilter2D enemyFilter = new ContactFilter2D();
            enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            Physics2D.OverlapCircle(transform.position, pushRadius, enemyFilter, nearby);
            if (!hop || moving)
            {
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
            if(hop)
            {
                if (!moving)
                {
                    rb.velocity = Vector2.zero;
                    if (shootState == 1)
                    {
                        GetComponent<BasicEnemyFire>().firing = true;
                    }
                    else if (shootState == 2)
                    {
                        GetComponent<BasicEnemyFire>().firing = false;
                    }
                }
                else
                {
                    if (shootState == 1)
                    {
                        GetComponent<BasicEnemyFire>().firing = false;
                    }
                    else if(shootState == 2)
                    {
                        GetComponent<BasicEnemyFire>().firing = true;
                    }
                }
            }
        }
        else
        {
            target = patrol[patrolIndex].gameObject;
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

    private Vector2 OffsetBehavior(Vector2 direction, bool init = false)
    {
        if (init)
        {
            if (enemyType == "Seeker1" || enemyType == "Seeker2")
            {
                RandomOffset();
            }
            if (enemyType == "Slither")
            {
                offset = new Vector2(offset.x, Random.Range(-offset.y,offset.y));
                if(Random.Range(0,1) > .5)
                {
                    slitherDir = -slitherDir;
                }
            }
        }
        else
        {
            if (!IsInvoking("RandomOffset") && enemyType == "Seeker1")
            {
                Invoke("RandomOffset", Random.Range(0, 6));
            }
            if(enemyType == "Seeker2")
            {
                direction = direction - (Vector2)(Quaternion.AngleAxis(player.transform.rotation.eulerAngles.z, Vector3.forward) * offset) + offset;
                offset = Quaternion.AngleAxis(orbitRate, Vector3.forward) * offset;
            }
            if(enemyType == "Slither")
            {
                offset = new Vector2(offset.x, offset.y + slitherDir*orbitRate);
                if (offset.y > originalOffset.y)
                {
                    slitherDir = -slitherDir;
                    offset = new Vector2(offset.x, originalOffset.y);
                }
                else if(offset.y < -originalOffset.y)
                {
                    slitherDir = -slitherDir;
                    offset = new Vector2(offset.x, -originalOffset.y);
                }
            }
        }
        return direction;
    }

    private void RandomOffset()
    {
        offset = Quaternion.AngleAxis(Random.Range(0,360), Vector3.forward) * offset;
    }
}
