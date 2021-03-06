﻿using System.Collections;
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
    internal PopupMenu menu;

    // Use this for initialization
    void Start()
    {
        menu = FindObjectOfType<PopupMenu>();
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
            //print("E down");
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
                //print("Interactable detects E");
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
        if (phrases[1][0] == '!')
        {
            string newLine = phrases[0];
            newLine = newLine + " " + phrases[1].Substring(1);
            for(int i = 2;i<phrases.Length;++i)
            {
                newLine += " " + phrases[i];
            }
            return !CheckTruth(newLine);
        }
        if(phrases[1][0] == '_')
        {
            Item item = ScriptableObject.CreateInstance<Item>();
            item.title = phrases[1].Substring(1);
            return dictionary.Items.Contains(item);
        }
        if (phrases.Length == 2)
        {
            if (dictionary.BooleanDictionary.ContainsKey(phrases[1]))
            {
                booleanVal = dictionary.BooleanDictionary[phrases[1]];
            }
            else
            {
                return false;
            }
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
                menu.AddItem(phrases[1]);
            }
            else if (phrases[0] == "RemItem")
            {
                menu.RemoveItem(phrases[1]);
            }
            else if (phrases[0] == "AddQuest")
            {
                menu.AddQuest(phrases[1]);
                dictionary.BooleanDictionary[phrases[1] + "R"] = true;
                print(dictionary.BooleanDictionary[phrases[1] + "R"]);
            }
            else if (phrases[0] == "RemQuest")
            {
                menu.RemoveQuest(phrases[1]);
                dictionary.BooleanDictionary[phrases[1] + "C"] = true;
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
