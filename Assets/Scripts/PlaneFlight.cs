using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFlight : MonoBehaviour {

	public float riseSpeed;
	public float turnSpeed;
	public float minSpeed;
	public float maxSpeed;
	public float acceleration;
    public float forwardVelocity;
    public float dodgeLength;
    float trueMinSpeed;
    float trueMaxSpeed;
    bool midDodge;
	bool rising;
	Animator anim;
	PlaneManagement planeMan;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        trueMinSpeed = minSpeed;
        trueMaxSpeed = maxSpeed;
        anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
        forwardVelocity = minSpeed;
		rb.velocity = transform.right * forwardVelocity;
		planeMan = FindObjectOfType<PlaneManagement> ();
        Invoke("CheckAgain", .05f);
        midDodge = false;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (planeMan.fighting && !rising && transform.position.z > -planeMan.shift+.1f)
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,0) + planeMan.shift * Vector3.back;
        }
		if (Input.GetKey (KeyCode.W))
		{
			transform.position += Vector3.back*riseSpeed/2;
		}	
		if (Input.GetKey (KeyCode.S))
		{
			if (transform.position.z < 0)
			{
				transform.position += Vector3.forward*riseSpeed;
			}
		}
        //rb.velocity = transform.right * forwardVelocity + transform.up * dodgeVelocity;
        rb.velocity = transform.right * forwardVelocity;
		if (transform.position.z >= 0)
		{
			transform.position = (Vector2) transform.position;
		}

		if (Input.GetKey (KeyCode.LeftArrow))
		{
			transform.Rotate (new Vector3(0,0,turnSpeed));
		}
		if (Input.GetKey (KeyCode.RightArrow))
		{
			transform.Rotate (new Vector3(0,0,-turnSpeed));
		}
		if (Input.GetKey (KeyCode.UpArrow))
		{
			forwardVelocity += acceleration;
		}
		if (Input.GetKey (KeyCode.DownArrow))
		{
			forwardVelocity -= acceleration;
		}
        forwardVelocity = Mathf.Clamp(forwardVelocity, minSpeed, maxSpeed);
		if (Input.GetKey (KeyCode.Q))
		{
			if (!midDodge)
			{
				anim.SetBool ("DodgeL", true);
                StartCoroutine(Dodge("Left"));
			}
		}
		if (Input.GetKey (KeyCode.E))
		{
			if (!midDodge)
			{
				anim.SetBool ("DodgeR", true);
                StartCoroutine(Dodge("Right"));
            }
        }
	}

	public IEnumerator ChangeStrata(int direction)
	{
		print("Changing strata");
        rising = true;
		float newHeight = Mathf.RoundToInt ((transform.position.z + planeMan.shift) / planeMan.strata) * planeMan.strata + direction*planeMan.strata-planeMan.shift;
		float wiggle = .3f;
		while(transform.position.z > newHeight + wiggle || transform.position.z < newHeight - wiggle)
		{
			transform.position += Vector3.forward*riseSpeed*direction;
			yield return new WaitForSecondsRealtime(.02f);
		}
		transform.position = new Vector3 (transform.position.x, transform.position.y, Mathf.RoundToInt((transform.position.z + planeMan.shift) / planeMan.strata) * planeMan.strata - planeMan.shift);
        rising = false;
	}

    public IEnumerator Dodge(string direction)
    {
        midDodge = true;
        GetComponent<Health>().StartInvincibility(dodgeLength);
        maxSpeed = maxSpeed * 2;
        minSpeed = minSpeed * 2;
        forwardVelocity = maxSpeed;
        yield return new WaitForSecondsRealtime(dodgeLength);
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