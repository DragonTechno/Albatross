using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// all objects with dialog and interaction on the isometric level
public class Interactable : MonoBehaviour
{
    public GameObject player;
    public GlobalDictionary dictionary;
    public float nearDistance;
    internal InteractionLock Lock; // makes sure you're only talking to one person
    internal SpriteRenderer thisSprite;
    internal Color originalColor;

    // Use this for initialization
    void Start()
    {
        dictionary = FindObjectOfType<GlobalDictionary>();
        thisSprite = GetComponent<SpriteRenderer>();
        Lock = FindObjectOfType<Canvas>().GetComponentInChildren<InteractionLock>();
        originalColor = thisSprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("E down");
        }
        InteractionUpdate();
    }

    internal void InteractionUpdate()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < nearDistance)
        {
            thisSprite.color = Color.yellow;
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Interactable detects E");
                if (!Lock.locked || Lock.connectedObject == this)
                {
                    Lock.SetLock(this);
                    InteractionAction();
                }
            }
        }
        else
        {
            thisSprite.color = originalColor;
        }
    }

    virtual public void InteractionAction()
    {

    }

    internal bool CheckTruth(string startLine)
    {
        string[] phrases = startLine.Split();
        bool booleanVal;
        if (phrases.Length == 2)
        {
            booleanVal = dictionary.BooleanDictionary[phrases[1]];
        }
        else
        {
            int dictVar = dictionary.IntDictionary[phrases[1]];
            int compVar = 0;
            int.TryParse(phrases[3], out compVar);
            if (phrases[2] == ">")
            {
                booleanVal = (dictVar > compVar);
            }
            else if (phrases[2] == ">=")
            {
                booleanVal = (dictVar >= compVar);
            }
            else if (phrases[2] == "<")
            {
                booleanVal = (dictVar < compVar);
            }
            else if (phrases[2] == "<=")
            {
                booleanVal = (dictVar <= compVar);
            }
            else
            {
                booleanVal = (dictVar == compVar);
            }
        }
        return booleanVal;
    }

    public void RunString(List<string> scripts) //Run dialog scripts based on their names
    {
        foreach (string script in scripts)
        {
            print(script);
            string tempScript = script.Replace("\n", string.Empty);
            string[] phrases = tempScript.Split(' ');
            if (phrases[0] == "AddItem")
            {
                FindObjectOfType<PopupMenu>().AddItem(phrases[1]);
            }
            else if (phrases[0] == "RemItem")
            {
                FindObjectOfType<PopupMenu>().RemoveItem(phrases[1]);
            }
            else if (phrases[0] == "AddQuest")
            {
                FindObjectOfType<PopupMenu>().AddQuest(phrases[1]);
            }
            else if (phrases[0] == "RemQuest")
            {
                FindObjectOfType<PopupMenu>().RemoveQuest(phrases[1]);
            }
            else if (phrases.Length == 1)
            {
                Invoke(phrases[0], 0);
            }
            else
            {
                print("Behavior undefined");
            }
        }
    }
}
