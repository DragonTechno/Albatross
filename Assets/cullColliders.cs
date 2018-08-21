using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cullColliders : MonoBehaviour {

    GameObject plane;
    SpriteRenderer sprite;
    Collider2D coll;
    float cullingProportion = 6;
    bool active;

    // Use this for initialization
    void Start () {
        active = false;
        cullingProportion = 15;
        plane = FindObjectOfType<PlaneManagement>().plane;
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        float cullingDistance = Camera.main.orthographicSize*cullingProportion;
        float distance = ((Vector2)transform.position - (Vector2)plane.transform.position).magnitude - Mathf.Max(sprite.bounds.extents.x,sprite.bounds.extents.y);
        if(distance <= cullingDistance && !active)
        {
            print("Uncull " + gameObject.name);
            coll.enabled = true;
            active = true;
            sprite.enabled = true;
            print("Finished unculling " + gameObject.name);
        }
        else if(distance > cullingDistance && active)
        {
            print("Cull " + gameObject.name);
            coll.enabled = false;
            active = false;
            sprite.enabled = false;
            print("Finished culling " + gameObject.name);
        }
    }
}
