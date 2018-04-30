using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

	public GameObject Target;
	public float hoverHeight;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Target)
        {
            transform.position = Target.transform.position + Vector3.back * hoverHeight;
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
}
