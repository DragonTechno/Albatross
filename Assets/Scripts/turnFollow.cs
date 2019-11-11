using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class turnFollow : MonoBehaviour {

	public float turnSpeed; //Maximum angular velocity of the ship
    public float orbitRate=1; //If the enemy follows a position orbiting the player, this is the rate at which that position rotates.
    public float orbitRadius; //For orbiting follow paths

    public float speed; //Maximum forward velocity of the ship
    public float stopability = 0f; //From 0 to 1: By what proportion an enemy will slow down and turn if they overshoot the player.
	public float followDistance; //Fight distance of the ship.
    public float aggroDistance;
    public float followHeight; //The max height difference at which an enemy will "see" the player. (REMOVE)
    public float riseSpeed; //How fast the enemy changes height (REMOVE)
    public float wayPointDis; //How close this has to get to a waypoint before switching to a different one

    public float basePushForce = 5f; //How much this unit pushes away nearby similar units (to space out).
    public float pushRadius = 10f; //Radius at which similar units are considered "nearby"

    public Vector2 offset; //Specifies a position offset from the player to follow (also the center of an orbit)
    
	public string enemyType; //Type of the ship (currently just used for pushing)

    public bool followPlayer = true;
    public bool moveParent = false;
    public bool turn = true; //Whether the ship's sprite turns with velocity

    public int faceTarget; //0 = Don't face target, 1 = face player/direct target
    public bool rotateWithTarget; //If the player rotates, rotate your offset from them as well.
    public bool rotateInPlace; //For enemies that rotate in place (doesn't work with faceTarget)
    public bool rotateWhileStill; //Rotate while sitting still vs only while moving (for hopping)

    public float rotationSpeed; //Rotation speed for rotating in place
	public GameObject player; //The player to follow
    public patrolHolder pHolder; //Script that manages an enemy's default partol path
    public GameObject target; //The object it currently follows
    public bool forceOutOfFight = false; //Forces camera wide when near

    //Variables defining hopping behavior
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
    void OnEnable() {
        if (GetComponent<TrailRenderer>())
        {
            GetComponent<TrailRenderer>().Clear();
        }
        hopDelay = Random.Range(0,hopWaitTime);
        EnemyFire = GetComponent<BasicEnemyFire>();
        slitherDir = 1;

        if(player == null)
        {
            player = FindObjectOfType<PlaneFlight>().gameObject;
        }

        if (pHolder)
        {
            //print("Patrol extracted");
            patrol = pHolder.patrol;
            patrolIndex = 0;
            float nearestDistance = 1000f;
            for(int i = 0; i < patrol.Length; ++i)
            {
                if((transform.position-patrol[i].position).magnitude < nearestDistance)
                {
                    nearestDistance = (transform.position - patrol[i].position).magnitude;
                    patrolIndex = i;
                }
            }
            target = patrol[patrolIndex].gameObject;
        }
        else if(followPlayer && player)
        {
            target = player;
        }
        else if(!target)
        {
            target = gameObject;
        }

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
        UpdateOffsets(true);
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        UpdateOffsets();
        if (target)
        {
            float pushForce = basePushForce;
            Vector2 distance = Vector2.zero;
            //float heightDiff = 1000;

            if(hop)
            {
                hopDelay += Time.fixedDeltaTime;
            }

            if (target && faceTarget == 1)
            {
                transform.right = target.transform.position - transform.position;
            }

            bool isAggro = (((transform.position-player.transform.position).magnitude < followDistance && target == player) ||
                ((transform.position - player.transform.position).magnitude < aggroDistance));

            if (followPlayer && isAggro && player /*&& !forceOutOfFight*/)
            {
                target = player;
                pushForce = pushForce * 2;
                distance = GetFollowPosition();

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

                if (faceTarget == 1)
                {
                    transform.right = player.transform.position - transform.position;
                }

            }
            else
            {
                if(pHolder)
                {
                    if (fighting)
                    {
                        patrol = pHolder.patrol;
                        patrolIndex = 0;
                        float nearestDistance = 1000f;
                        for (int i = 0; i < patrol.Length; ++i)
                        {
                            if ((transform.position - patrol[i].position).magnitude < nearestDistance)
                            {
                                nearestDistance = (transform.position - patrol[i].position).magnitude;
                                patrolIndex = i;
                            }
                        }
                        target = patrol[patrolIndex].gameObject;
                    }
                    else
                    {
                        target = patrol[patrolIndex].gameObject;
                    }
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

                distance = (Vector2)target.transform.position - (Vector2)transform.position;
                if (pHolder && distance.magnitude < wayPointDis)
                {
                    patrolIndex = (patrolIndex + 1) % patrol.Length;
                }

            }

            if (turn)
            {

                if (!hop || moving)
                {
                    float angle = Vector2.SignedAngle(transform.right, distance);

                    // right keeps track of where the pointer is pointing to, or where its right side points to
                    Quaternion rotationQ = Quaternion.AngleAxis(Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), Mathf.Abs(turnSpeed) * Random.Range(-.4f, 2f)), Vector3.forward);
                    transform.right = rotationQ * transform.right;

                    //turnFromWall();

                    // the math part of this line makes sure that enemies goes faster when they're further away and is capped
                    rb.AddForce(transform.right * speed * Mathf.Clamp(Mathf.Sqrt(distance.magnitude / 5f)-stopability/2, 0, 1.5f) * (1-Mathf.Abs(angle)*Mathf.Clamp(stopability,0f,1f)/180));
                    //rb.velocity = (Vector2.Dot(rb.velocity.normalized * Mathf.Clamp(rb.velocity.magnitude,speed,speed*1.5f), transform.right))*transform.right;
                }

            }
            else
            {
                rb.AddForce(distance.normalized * speed * Mathf.Clamp(Mathf.Sqrt(distance.magnitude / 5), .5f, 1.5f));
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
                    rb.velocity = Vector2.zero;
                    hopDelay = 0;
                }

            }

            //// an empty list that we'll add to if there are any overlapping enemies
            //Collider2D[] nearby = new Collider2D[10];

            //// filter states that we're only looking for objects with the enemy layer
            //ContactFilter2D enemyFilter = new ContactFilter2D();
            //enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            //enemyFilter.useLayerMask = true;
            //enemyFilter.useTriggers = true;
            //Physics2D.OverlapCircle(transform.position, pushRadius, enemyFilter, nearby);

            //if (!hop || moving)
            //{
            //    foreach (Collider2D collider in nearby)
            //    {
            //        if (collider)
            //        {
            //            turnFollow unit = collider.gameObject.GetComponent<turnFollow>();
            //            if (unit && unit != this)
            //            {
            //                // calculate direction from target to me
            //                if (enemyType == unit.enemyType)
            //                {
            //                    Vector2 bvec = transform.position - unit.gameObject.transform.position;
            //                    bvec = bvec.normalized * Mathf.Clamp(pushForce / bvec.magnitude, 1, basePushForce*20);
            //                    rb.AddForce(bvec);
            //                }
            //            }
            //        }
            //    }
            //}

            if(hop)
            {
                Animator thisAnimator = GetComponent<Animator>();
                if (!moving)
                {
                    rb.velocity = Vector2.zero;
                    
                    if(thisAnimator)
                    {
                        thisAnimator.SetBool("Hopping", false);
                    }

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

                    if (thisAnimator)
                    {
                        thisAnimator.SetBool("Hopping", true);
                    }

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
                patrol = pHolder.patrol;
                patrolIndex = 0;
                float nearestDistance = 1000f;
                for (int i = 0; i < patrol.Length; ++i)
                {
                    if ((transform.position - patrol[i].position).magnitude < nearestDistance)
                    {
                        nearestDistance = (transform.position - patrol[i].position).magnitude;
                        patrolIndex = i;
                    }
                }
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

    void turnFromWall()
    {
        ContactFilter2D wallFilter = new ContactFilter2D();
        wallFilter.SetLayerMask(LayerMask.GetMask("Default"));
        wallFilter.useLayerMask = true;
        wallFilter.useTriggers = false;
        RaycastHit2D[] wallCheck = new RaycastHit2D[1];
        RaycastHit2D[] leftCheck = new RaycastHit2D[1];
        RaycastHit2D[] rightCheck =new RaycastHit2D[1];

        Physics2D.Raycast(transform.position, transform.right, wallFilter, wallCheck, 10f);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.forward) * transform.right, wallFilter, leftCheck, 10f);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.forward)*transform.right, wallFilter, rightCheck, 10f);
        Debug.DrawLine(transform.position, transform.position + transform.right*10f);

        if(wallCheck[0])
        {
            Debug.DrawLine(wallCheck[0].point, wallCheck[0].point - wallCheck[0].normal * 5f, Color.red);
            float pathCorrectionAngle = Vector2.SignedAngle(-wallCheck[0].normal, transform.right);
            transform.right = Quaternion.AngleAxis(4*Mathf.Sign(pathCorrectionAngle)*turnSpeed * (1 - Mathf.Abs(pathCorrectionAngle) / 180), Vector3.forward) * transform.right;
            rb.velocity = transform.right * rb.velocity.magnitude;
            //rb.velocity = rb.velocity * (1 - stopability * (1 - Mathf.Clamp(Mathf.Abs(pathCorrectionAngle)/90,0f,1f)));
        }
        else
        {
            if(leftCheck[0])
            {
                transform.right = Quaternion.AngleAxis(2 * turnSpeed, Vector3.forward) * transform.right;
            }
            if(rightCheck[0])
            {
                transform.right = Quaternion.AngleAxis(-2 * turnSpeed, Vector3.forward) * transform.right;
            }
        }

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
    private Vector2 GetFollowPosition(GameObject tempTarget = null)
    {
        if(tempTarget == null)
        {
            tempTarget = target;
        }
        Vector2 direction = new Vector2(0,0);
        if (rotateWithTarget)
        {
            direction = tempTarget.transform.position + (Quaternion.AngleAxis(tempTarget.transform.rotation.eulerAngles.z, Vector3.forward) * offset) - transform.position;
        }
        else
        {
            direction = tempTarget.transform.position + (Vector3)offset - transform.position;
        }
        return direction;
    }

    //Seeker1 jumps to follow a random position a certain distance away from the player
    //Seeker2 follows a constantly rotating orbit about that player.
    //Slither swims back and forth as it follows the player.
    private void UpdateOffsets(bool init = false)
    {
        if (init)
        {
            if (enemyType == "Seeker1" || enemyType == "Seeker2")
            {
                transform.right = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector2.up;
                RandomOffset();
            }

            if (enemyType == "Slither")
            {

                offset = new Vector2(offset.x, Random.Range(-offset.y, offset.y));
                if (Random.Range(0, 1) > .5)
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

            if (enemyType == "Seeker2")
            {
                orbitPosition = Quaternion.AngleAxis(orbitRate, Vector3.forward) * orbitPosition;
                offset = originalOffset + orbitPosition;
            }

            if (enemyType == "Slither")
            {
                offset = new Vector2(offset.x, offset.y + slitherDir * orbitRate * (Mathf.Abs(Mathf.Abs(offset.y) - originalOffset.y) + .2f) / originalOffset.y);
                if (offset.y > originalOffset.y)
                {
                    slitherDir = -slitherDir;
                    offset = new Vector2(offset.x, originalOffset.y);
                }
                else if (offset.y < -originalOffset.y)
                {
                    slitherDir = -slitherDir;
                    offset = new Vector2(offset.x, -originalOffset.y);
                }
            }
        }
    }

    private void RandomOffset()
    {
        orbitPosition = (Quaternion.AngleAxis(Random.Range(0,360), Vector3.forward) * new Vector2(orbitRadius,0));
        offset = originalOffset + orbitPosition;
    }

    private void OnDrawGizmos()
    {
        if (rb != null && player)
        {
            Vector3 modifiedOffset = offset;
            if (rotateWithTarget)
            {
                modifiedOffset = (Quaternion.AngleAxis(target.transform.rotation.eulerAngles.z, Vector3.forward) * offset);
            }
            Vector2 destination = target.transform.position + modifiedOffset;
            if(target != player)
            {
                destination = target.transform.position;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(destination, 1f);
            Debug.DrawLine(transform.position, destination, Color.red);
            if(orbitRadius > 0 && target == player)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(target.transform.position + Quaternion.AngleAxis(target.transform.rotation.eulerAngles.z, Vector3.forward) * originalOffset, orbitRadius);
            }
            if (target == player)
            {
                Gizmos.color = new Color(1f, 0, 0, .2f);
                Gizmos.DrawSphere(transform.position, followDistance);
            }
            else
            {
                Gizmos.color = new Color(0, 0, 1f, .2f);
                Gizmos.DrawSphere(transform.position, aggroDistance);
            }
        }
    }
}
