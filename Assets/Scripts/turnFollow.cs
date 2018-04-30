using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnFollow : MonoBehaviour {

	public float turnSpeed; //Maximum angular velocity of the ship
	public float speed; //Maximum forward velocity of the ship
	public float followDistance; //Fight distance of the ship
	public string enemyType; //Type of the ship (currently unused)
	public GameObject target; //The object it follows
	PlaneManagement planeMan; //The plane data
	Rigidbody2D rb; //This object's rigid body

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		planeMan = FindObjectOfType<PlaneManagement> ();
    // for testing purposes, makes sure it's keeping track of the right plane manager
    Invoke("CheckAgain", .05f);
	}

	// Update is called once per frame
	void Update () {
		float pushRadius = 10f;
		float pushForce = 5f;
		Vector2 distance = target.transform.position - transform.position;
		if (distance.magnitude < followDistance)
		{
			pushForce = pushForce * 2;
			float HeightFollowRate = 6f;
			if (!IsInvoking ("FollowHeight"))
			{
				InvokeRepeating ("FollowHeight", 0, HeightFollowRate);
			}
      print("Fight request.");
			planeMan.RequestFight ();
      // .right keeps track of where the pointer is pointing to, or where its right side points to
			transform.right = (Vector2)Vector3.RotateTowards (transform.right, distance, turnSpeed*Random.Range(.7f,1.4f), turnSpeed);
      // the math part of this line makes sure that enemies goes faster when they're further away and is capped
      rb.velocity = transform.right * speed * Mathf.Clamp (Mathf.Sqrt (distance.magnitude / 10), 1, 1.5f);
		}
		else
		{
			CancelInvoke ("FollowHeight");
		}
    // an empty list that we'll add to if there are any overlapping enemies
		Collider2D[] nearby = new Collider2D[10];
    // filter states that we're only looking for objects with the enemy layer
		ContactFilter2D enemyFilter = new ContactFilter2D ();
		enemyFilter.SetLayerMask (LayerMask.GetMask("Enemy"));
		Physics2D.OverlapCircle (transform.position, pushRadius, enemyFilter, nearby);
		foreach (Collider2D collider in nearby)
		{
			if (collider)
			{
				if (collider.gameObject.GetComponent<turnFollow> ())
				{
					turnFollow unit = collider.attachedRigidbody.gameObject.GetComponent<turnFollow> ();
					// calculate direction from target to me
					if (enemyType == unit.enemyType)
					{
						Vector2 bvec = transform.position - unit.gameObject.transform.position;
						bvec = bvec.normalized * Mathf.Clamp (pushForce / bvec.magnitude, 1, 100);
						rb.AddForce (bvec);
					}
				}
			}
		}
	}

	void FollowHeight()
	{
		float strataHeight = Mathf.Round (transform.position.z / planeMan.strata);
		float targetHeight = Mathf.Round (target.transform.position.z / planeMan.strata);
		if (targetHeight != strataHeight)
		{
			if(targetHeight < strataHeight)
			{
				transform.position += Vector3.back * planeMan.strata;
			}
			else
			{
				transform.position += Vector3.forward * planeMan.strata;
			}
		}
	}

    void CheckAgain()
    {
        planeMan = FindObjectOfType<PlaneManagement>();
    }
}
