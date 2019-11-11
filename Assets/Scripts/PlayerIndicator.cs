using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject player;
    public float colorModifier;
    public float scaleModifier;
    SpriteRenderer mySprite;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player)
        {
            float diff = Mathf.Abs(mainCam.transform.position.z - player.transform.position.z);
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, Mathf.Max(0f, diff - colorModifier) / (diff + colorModifier*3f));
            float newScale = Mathf.Clamp(scaleModifier / (diff), 0f, 100);
            mySprite.transform.localScale = new Vector3(newScale, newScale, 1);
            transform.rotation = player.transform.rotation;
        }
    }
}
