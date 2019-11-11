using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFlight : MonoBehaviour {

	//public float riseSpeed;
	public float turnSpeed;
	public float minSpeed;
	public float maxSpeed;
	public float acceleration;
    public float dodgeSpeedMultiplier;
    public float dodgeLength;
    public float dodgeDelay;
    public float maxBoost;
    public float boostRecoverySpeed;
    public float dodgeCost;
    public bool turnAccel;
    internal float boost;
    float trueMinSpeed;
    float trueMaxSpeed;
    float forwardVelocity;
    bool midDodge;
	//bool rising;
	Animator anim;
	PlaneManagement planeMan;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        boost = maxBoost;
        trueMinSpeed = minSpeed;
        trueMaxSpeed = maxSpeed;
        anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
        forwardVelocity = maxSpeed;
		rb.velocity = transform.right * forwardVelocity;
		planeMan = FindObjectOfType<PlaneManagement> ();
        Invoke("CheckAgain", .05f);
        midDodge = false;
    }

    // Update is called once per frame
    void FixedUpdate () {
        boost = Mathf.Clamp(boost + boostRecoverySpeed, 0, maxBoost);
        if (planeMan.fighting && transform.position.z > -planeMan.shift+.1f)
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,0) + planeMan.shift * Vector3.back;
        }
		//if (Input.GetKey (KeyCode.Z))
		//{
		//	transform.position += Vector3.back*riseSpeed/2;
		//}	
		//if (Input.GetKey (KeyCode.X))
		//{
		//	if (transform.position.z < 0)
		//	{
		//		transform.position += Vector3.forward*riseSpeed;
		//	}
		//}
        //rb.velocity = transform.right * forwardVelocity + transform.up * dodgeVelocity;
        rb.velocity = transform.right * forwardVelocity;
        if (transform.position.z >= 0)
        {
            transform.position = (Vector2)transform.position;
        }

        if (turnAccel)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, 0, turnSpeed));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, 0, -turnSpeed));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                forwardVelocity += acceleration;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                forwardVelocity -= acceleration;
            }
        }
        else
        {
            Vector2 goalDirection = Vector2.zero;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                goalDirection += Vector2.left;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                goalDirection += Vector2.right;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                goalDirection += Vector2.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                goalDirection += Vector2.down;
            }

            if (goalDirection != Vector2.zero)
            {
                float angle = Vector2.SignedAngle(transform.right, goalDirection);
                if (Mathf.Abs(angle) < 160)
                {
                    // right keeps track of where the pointer is pointing to, or where its right side points to
                    //Quaternion rotationQ = Quaternion.AngleAxis(Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), Mathf.Abs(turnSpeed)), Vector3.forward);
                    transform.Rotate(new Vector3(0, 0, Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), Mathf.Abs(turnSpeed))));
                }
            }
        }

        forwardVelocity = Mathf.Clamp(forwardVelocity, minSpeed, maxSpeed);
		if (Input.GetKey (KeyCode.LeftShift) && boost >= dodgeCost)
		{
			if (!midDodge)
			{
				anim.SetBool ("DodgeL", true);
                StartCoroutine(Dodge("Left"));
			}
		}
		if (Input.GetKey (KeyCode.RightShift) && boost >= dodgeCost)
		{
			if (!midDodge)
			{
				anim.SetBool ("DodgeR", true);
                StartCoroutine(Dodge("Right"));
            }
        }
	}

	//public IEnumerator ChangeStrata(int direction)
	//{
	//	print("Changing strata");
 //       rising = true;
	//	float newHeight = Mathf.RoundToInt ((transform.position.z + planeMan.shift) / planeMan.strata) * planeMan.strata + direction*planeMan.strata-planeMan.shift;
	//	float wiggle = .3f;
	//	while(transform.position.z > newHeight + wiggle || transform.position.z < newHeight - wiggle)
	//	{
	//		transform.position += Vector3.forward*riseSpeed*direction;
	//		yield return new WaitForSecondsRealtime(.02f);
	//	}
	//	transform.position = new Vector3 (transform.position.x, transform.position.y, Mathf.RoundToInt((transform.position.z + planeMan.shift) / planeMan.strata) * planeMan.strata - planeMan.shift);
 //       rising = false;
	//}

    public IEnumerator Dodge(string direction)
    {
        boost -= dodgeCost;
        midDodge = true;
        GetComponent<Health>().StartInvincibility(dodgeLength);
        maxSpeed = maxSpeed * dodgeSpeedMultiplier;
        minSpeed = minSpeed * dodgeSpeedMultiplier;
        forwardVelocity = maxSpeed;
        yield return new WaitForSeconds(dodgeLength+dodgeDelay);
        midDodge = false;
        minSpeed = trueMinSpeed;
        maxSpeed = trueMaxSpeed;
    }

	public void EndDodge()
	{
		anim.SetBool ("DodgeR", false);
		anim.SetBool ("DodgeL", false);
	}

    void CheckAgain()
    {
        planeMan = FindObjectOfType<PlaneManagement>();
    }
}