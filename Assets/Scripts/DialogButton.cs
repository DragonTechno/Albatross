using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogButton : MonoBehaviour {
    Interactable connectedDialog;
    string scriptString;

    public void Initialize(string scriptString, Interactable connectedDialog, string text)
    {
        this.scriptString = scriptString;
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
        print(scriptString + " from a button");
        connectedDialog.RunString(scriptString);
    }

    public void DestroyButtons ()
    {
        DialogButton[] Buttons = transform.parent.gameObject.GetComponentsInChildren<DialogButton>();
        foreach(DialogButton choice in Buttons)
        {
            Destroy(choice.gameObject);
        }
        connectedDialog.Activate();
        connectedDialog.buttonH = 0;
    }
}
