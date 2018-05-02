﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogButton : MonoBehaviour {
    Interactable connectedDialog;
    List<string> scriptText;

    public void Initialize(List<string> scriptText, Interactable connectedDialog, string text)
    {
        this.scriptText = scriptText;
        this.connectedDialog = connectedDialog;
        Button parentButton = GetComponent<Button>();
        parentButton.GetComponentInChildren<Text>().text = text;
        parentButton.onClick.AddListener(ChangeScript);
        parentButton.onClick.AddListener(DestroyButtons);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeScript()
    {
        print(scriptText + " from a button");
        connectedDialog.RunString(scriptText);
    }

    public void DestroyButtons ()
    {
        DialogButton[] Buttons = transform.parent.gameObject.GetComponentsInChildren<DialogButton>();
        foreach(DialogButton choice in Buttons)
        {
            Destroy(choice.gameObject);
        }
        connectedDialog.Activate();
    }
}
