using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullChunk : MonoBehaviour {

    GameObject plane;
    public float cullingDistance;
    bool culled;
    CullingGroup group;

	// Use this for initialization
	void Start () {
        plane = FindObjectOfType<PlaneManagement>().plane;
        culled = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        Vector2 difference = transform.position - plane.transform.position;
        float distance = difference.magnitude;
        if (distance < cullingDistance && culled)
        {
            culled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        if (distance > cullingDistance && !culled)
        {
            culled = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (plane)
        {
            Vector2 difference = transform.position - plane.transform.position;
            float distance = difference.magnitude;
            if (distance < cullingDistance && culled)
            {
                print("Loading chunk");
                culled = false;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            if (distance > cullingDistance && !culled)
            {
                print("Culling chunk");
                culled = true;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
