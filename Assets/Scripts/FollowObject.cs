using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

	public GameObject Target;
	public float hoverHeight;
    public float distantHeight;
    public float cameraChangeSpeed;
    PlaneManagement planeMan;
    Vector3 offset;
    bool moving;
    bool aggro;
    IEnumerator activeCoroutine;

	// Use this for initialization
	void Start () {
        moving = false;
        aggro = false;
        offset = Vector3.back * hoverHeight;
        planeMan = FindObjectOfType<PlaneManagement>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Target)
        {
            transform.position = Target.transform.position + offset;
        }
        if(!planeMan.fighting)
        {
            if (offset != Vector3.back * distantHeight && aggro)
            {
                if(moving)
                {
                    StopCoroutine(activeCoroutine);
                }
                moving = true;
                print("Start rise");
                activeCoroutine = AccelerateToOffset(Vector3.back * distantHeight);
                StartCoroutine(activeCoroutine);
                aggro = false;
            }
        }
        else
        {
            if(offset != Vector3.back * hoverHeight && !aggro)
            {
                if (moving)
                {
                    StopCoroutine(activeCoroutine);
                    moving = false;
                }
                print("Start descent");
                moving = true;
                activeCoroutine = AccelerateToOffset(Vector3.back * hoverHeight);
                StartCoroutine(activeCoroutine);
                aggro = true;
            }
        }
        if(Input.GetKey(KeyCode.W))
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            moving = false;
            if (Input.GetKey(KeyCode.W))
            {
                offset += Vector3.forward * cameraChangeSpeed;
            }
        }
        else if(Input.GetKey(KeyCode.S))
        {
            if(activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            moving = false;
            if (Input.GetKey(KeyCode.S))
            {
                offset -= Vector3.forward * cameraChangeSpeed;
            }
        }
	}

    public void SwitchFocus(GameObject newTarget)
    {
        StopAllCoroutines();
        StartCoroutine(AccelerateBetween(newTarget));
    }

    IEnumerator AccelerateBetween(GameObject newTarget)
    {
        Target = null;
        float flatSpeed = .02f;
        Vector2 target = newTarget.transform.position;
        while ((Vector2) transform.position != (Vector2) target)
        {
            target = newTarget.transform.position;
            float height = transform.position.z;
            Vector3 newPos = (Vector3) Vector2.MoveTowards((Vector2) transform.position, target, Vector2.Distance(transform.position, target)/10 + flatSpeed);
            newPos = newPos + height * Vector3.forward;
            transform.position = newPos;
           /* print("New Position: " + newPos);
            print("Current Position: " + transform.position);
            print("Target Position: " + target); */
            yield return null;
        }
        Target = newTarget;
    }

    IEnumerator AccelerateToOffset(Vector3 newOffset)
    {
        float flatSpeed = .03f;
        while (offset != newOffset)
        {
            offset = Vector3.MoveTowards(offset, newOffset, Mathf.Clamp(Mathf.Log10(Vector3.Distance(offset, newOffset)) + flatSpeed, flatSpeed, 10000));
            yield return null;
        }
        moving = false;
        activeCoroutine = null;
    }
}
