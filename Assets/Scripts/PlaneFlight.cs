using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFlight : MonoBehaviour {

	public float riseSpeed;
	public float turnSpeed;
	public float minSpeed;
	public float maxSpeed;
	public float acceleration;
	bool rising;
	Animator anim;
	PlaneManagement planeMan;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		rb.velocity = transform.right * minSpeed;
		planeMan = FindObjectOfType<PlaneManagement> ();
        Invoke("CheckAgain", .05f);
    }

    // Update is called once per frame
    void Update () {
        if (planeMan.fighting)
		{
            if (!rising)
            {
                if (transform.position.z > -planeMan.shift+.1f)
                {
                    transform.position = new Vector3(transform.position.x,transform.position.y,0) + planeMan.shift * Vector3.back;
                }
                else
                {
                    float height = Mathf.RoundToInt((transform.position.z+planeMan.shift) / planeMan.strata) * planeMan.strata - planeMan.shift;
                    transform.position = new Vector3(transform.position.x, transform.position.y, height);
                }
            }
			if (Input.GetKeyDown(KeyCode.W))
			{
				StartCoroutine(ChangeStrata(1));
			}	
			if (Input.GetKeyDown (KeyCode.S))
			{
				if (transform.position.z < 0)
				{
					StartCoroutine(ChangeStrata(-1));
				}
			}
		}
		else
		{
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
		}
		rb.velocity = transform.right * rb.velocity.magnitude;
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
			rb.velocity += (Vector2) transform.right * acceleration;
			if (Vector2.Dot(rb.velocity,transform.right)/transform.right.magnitude  > maxSpeed)
			{
				rb.velocity = rb.velocity.normalized * maxSpeed;
			}
		}
		if (Input.GetKey (KeyCode.DownArrow))
		{
			rb.velocity -= (Vector2) transform.right * acceleration;
			if (Vector2.Dot(rb.velocity,transform.right)/transform.right.magnitude < minSpeed)
			{
				rb.velocity = rb.velocity.normalized * minSpeed;
			}
		}
		if (Input.GetKey (KeyCode.Q))
		{
			/*if (!anim.GetBool ("DodgeL") && !anim.GetBool ("DodgeR"))
			{
				anim.SetBool ("DodgeL", true);
			}*/
		}
		if (Input.GetKey (KeyCode.E))
		{
			/*if (!anim.GetBool ("DodgeL") && !anim.GetBool ("DodgeR"))
			{
				anim.SetBool ("DodgeR", true);
			}*/
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

	public IEnumerator Invincible(float duration)
	{
        GetComponent<Collider2D>().enabled = false;
		yield return new WaitForSecondsRealtime(duration);
		GetComponent<Collider2D>().enabled = true;
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
