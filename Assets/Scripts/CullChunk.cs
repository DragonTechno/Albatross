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
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        Vector2 difference = transform.position - plane.transform.position;
        float distance = difference.magnitude;
        if (distance < cullingDistance && culled)
        {
            culled = false;
            for (int i = 0; i < childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        if (distance > cullingDistance && !culled)
        {
            culled = true;
            for (int i = 0; i < childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
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
                int childCount = transform.childCount;
                culled = false;
                for (int i = 0; i < childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            if (distance > cullingDistance && !culled)
            {
                print("Culling chunk");
                int childCount = transform.childCount;
                culled = true;
                for (int i = 0; i < childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
