using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cullColliders : MonoBehaviour {

    GameObject plane;
    Renderer[] renderers;
    Collider2D coll;
    float cullingProportion = 6;
    bool active;
    SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        active = false;
        cullingProportion = 15;
        plane = FindObjectOfType<PlaneManagement>().plane;
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
        sprite = GetComponent<SpriteRenderer>();
        renderers = GetComponents<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update () {
        float cullingDistance = Camera.main.orthographicSize*cullingProportion;
        if (plane)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            float distance = ((Vector2)transform.position - (Vector2)plane.transform.position).magnitude - Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
            if (distance <= cullingDistance && !active)
            {
                //print("Uncull " + gameObject.name);
                coll.enabled = true;
                active = true;
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = true;
                }
                //print("Finished unculling " + gameObject.name);
            }
            else if (distance > cullingDistance && active)
            {
                //print("Cull " + gameObject.name);
                coll.enabled = false;
                active = false;
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = false;
                }
                //print("Finished culling " + gameObject.name);
            }
        }
    }
}
