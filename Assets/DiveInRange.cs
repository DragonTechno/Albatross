using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveInRange : MonoBehaviour
{
    public float diveDistance;
    public float diveRate;
    public float diveLength;
    public float diveForce;
    public GameObject player;
    public GameObject anchor;
    public float maxDistance;
    public bool constant = false;
    public bool stayWithAnchor;
    public bool diveInMaxDistance;
    public bool diving = false;
    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!player)
        {
            player = FindObjectOfType<PlaneFlight>().gameObject;
        }
        if(player && !constant)
        {
            StartCoroutine("Dive");
        }
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!initialized)
        {
            if (!player)
            {
                player = FindObjectOfType<PlaneFlight>().gameObject;
            }
            if (player && !constant)
            {
                StartCoroutine("Dive");
            }
            initialized = true;
        }
        float wiggle = 1f;
        if (player && (player.transform.position - transform.position).magnitude < diveDistance && (!diveInMaxDistance || (player.transform.position-anchor.transform.position).magnitude < maxDistance))
        {
            if (constant || diving)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                GetComponent<Rigidbody2D>().AddForce(direction * diveForce);
            }
        }
        else if (stayWithAnchor && (transform.position - anchor.transform.position).magnitude > wiggle)
        {
            if (constant || diving)
            {
                Vector2 direction = (anchor.transform.position - transform.position).normalized;
                GetComponent<Rigidbody2D>().AddForce(direction * diveForce);
            }
        }
        if ((transform.position - anchor.transform.position).magnitude > maxDistance && (transform.position + (Vector3)GetComponent<Rigidbody2D>().velocity.normalized * .1f - anchor.transform.position).magnitude > (transform.position - anchor.transform.position).magnitude)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    IEnumerator Dive()
    {
        while(true)
        {
            diving = true;
            yield return new WaitForSeconds(diveLength);
            diving = false;
            yield return new WaitForSeconds(Mathf.Clamp(diveRate - diveLength, 0, Mathf.Infinity));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(anchor.transform.position, maxDistance);
    }
}
