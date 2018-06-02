using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAheadOf : MonoBehaviour {

	public float maxDistance;
	public float followRate;
	public float followSpeed;
	public float rangeAhead;
    public float delay;
	public GameObject target;
	Vector2 startVector;
	Vector2 targetVector;
	bool following;
	float timer;

	// Use this for initialization
	void Start () {
        timer = delay;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (following)
		{
			if ((startVector - (Vector2) target.transform.position).magnitude < maxDistance && ((Vector2)transform.position != targetVector))
			{
				transform.position = Vector2.MoveTowards (transform.position, targetVector, followSpeed * Time.fixedDeltaTime);
			}
			else
			{
				following = false;
			}
		}
		else
		{
			if (timer < followRate)
			{
				timer += Time.fixedDeltaTime*Random.Range(.5f,1);
			}
			else
			{
				timer = 0;
				following = true;
				float range = 1f;
				targetVector = (Vector2) target.transform.position + target.GetComponentInParent<Rigidbody2D> ().velocity * rangeAhead + new Vector2(Random.Range(-range,range),Random.Range(-range,range));
				startVector = transform.position;
			}
		}
	}
}
