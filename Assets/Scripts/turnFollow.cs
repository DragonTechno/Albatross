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
    public float wayPointDis; //How close this has to get to a waypoint before switching to a different one
    public Vector2 offset;
    public float orbitRadius; //For orbiting follow paths
	public string enemyType; //Type of the ship (currently unused)
    public bool followPlayer = true;
    public bool moveParent = false;
    public bool turn = true; //Whether the ship's sprite turns with velocity
    public bool faceTarget;
    public bool rotateWithTarget;
    public bool rotateInPlace; //For enemies that rotate in place
    public bool rotateWhileStill;
    public float rotationSpeed; 
	public GameObject player; //The object it follows
    public patrolHolder pHolder;
    public GameObject target;
    public bool forceOutOfFight = false;

    [Header("Hopping Variables")]
    public bool hop = false;
    public float hopWaitTime;
    public float hopDuration;
    public int shootState = 0;//1=Attack while still, 2=Attack while moving
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
    Vector2 orbitPosition;
    int slitherDir;
    BasicEnemyFire EnemyFire;

	// Use this for initialization
	void Start () {
        hopDelay = Random.Range(0,hopWaitTime);
        EnemyFire = GetComponent<BasicEnemyFire>();
        slitherDir = 1;
        if (pHolder)
        {
            patrol = pHolder.patrol;
            target = patrol[0].gameObject;
        }
        else if(followPlayer && player)
        {
            target = player;
        }
        else if(!target)
        {
            target = gameObject;
        }
        patrolIndex = 0;
		rb = GetComponent<Rigidbody2D> ();
		planeMan = FindObjectOfType<PlaneManagement> ();
        if (followPlayer)
        {
            playerCollider = player.GetComponent<Collider2D>();
        }
        fighting = false;
        // for testing purposes, makes sure it's keeping track of the right plane manager
        Invoke("CheckAgain", .05f);
        if (followPlayer)
        {
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
        originalOffset = offset;
        moving = false;
        OffsetBehavior(Vector2.zero, true);
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
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
            if (followPlayer && player)
            {
                if (rotateWithTarget)
                {
                    distance = player.transform.position + (Quaternion.AngleAxis(player.transform.rotation.eulerAngles.z, Vector3.forward) * offset) - transform.position;
                }
                else
                {
                    distance = player.transform.position + (Vector3)offset - transform.position;
                }
                heightDiff = Mathf.Abs(player.transform.position.z - transform.position.z);
            }
            else
            {
                if (faceTarget)
                {
                    transform.right = target.transform.position - transform.position;
                }
            }
            if (followPlayer && distance.magnitude < followDistance && heightDiff < followHeight && player && !forceOutOfFight)
            {
                target = player;
                pushForce = pushForce * 2;
                distance = OffsetBehavior(distance);
                if (!fighting && followPlayer)
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
                if (faceTarget)
                {
                    transform.right = player.transform.position - transform.position;
                }
            }
            else
            {
                if(hop && !target)
                {
                    moving = false;
                    hopDelay = 0;
                }
                if(pHolder)
                {
                    target = patrol[patrolIndex].gameObject;
                }
                if (fighting)
                {
                    if (followPlayer && player)
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
                    }
                    planeMan.LeaveFight();
                    fighting = false;
                }
            }

            if (target != player)
            {
                distance = (Vector2)target.transform.position - (Vector2)transform.position;
                heightDiff = Mathf.Abs(target.transform.position.z - transform.position.z);
                if(pHolder && distance.magnitude < wayPointDis)
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
                    moving = false;
                    hopDelay = 0;
                }
                else if(!moving && hopDelay > hopWaitTime)
                {
                    moving = true;
                    hopDelay = 0;
                    if (EnemyFire)
                    {
                        GetComponent<BasicEnemyFire>().firing = true;
                    }
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
                    if (EnemyFire)
                    {
                        if (shootState == 1)
                        {
                            GetComponent<BasicEnemyFire>().firing = true;
                        }
                        else if (shootState == 2)
                        {
                            GetComponent<BasicEnemyFire>().firing = false;
                        }
                    }
                }
                else
                {
                    if (EnemyFire)
                    {
                        if (shootState == 1)
                        {
                            GetComponent<BasicEnemyFire>().firing = false;
                        }
                        else if (shootState == 2)
                        {
                            GetComponent<BasicEnemyFire>().firing = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (pHolder)
            {
                target = patrol[patrolIndex].gameObject;
            }
        }
        if(rotateInPlace)
        {
            if(fighting)
            {
                if((moving || rotateWhileStill) && !turn)
                {
                    transform.right = Quaternion.AngleAxis(rotationSpeed, Vector3.forward) * transform.right;
                }
            }
            else
            {
                transform.right = Quaternion.AngleAxis(rotationSpeed, Vector3.forward)*transform.right;
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

    //Seeker1 jumps to follow a random position a certain distance away from the player
    //Seeker2 follows a constantly rotating orbit about that player.
    //Slither swims back and forth as it follows the player.
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
                orbitPosition = Quaternion.AngleAxis(orbitRate, Vector3.forward) * orbitPosition;
                offset = originalOffset + orbitPosition;
                if (rotateWithTarget)
                {
                    direction = player.transform.position + (Quaternion.AngleAxis(player.transform.rotation.eulerAngles.z, Vector3.forward) * offset) - transform.position;
                }
                else
                {
                    direction = player.transform.position + (Vector3)offset - transform.position;
                }
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
        orbitPosition = (Quaternion.AngleAxis(Random.Range(0,360), Vector3.forward) * new Vector2(orbitRadius,0));
        transform.right = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector2.up;
        offset = originalOffset + orbitPosition;
    }
}
