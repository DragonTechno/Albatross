using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogButton : MonoBehaviour {
    Dialog connectedDialog;
    List<string> scriptText;

    public void Initialize(List<string> scriptText, Dialog connectedDialog, string text)
    {
        this.scriptText = scriptText;
        this.connectedDialog = connectedDialog;
        Button parentButton = GetComponent<Button>();
        if (text[0] == '$')
        {
            string imageName = text.Substring(1);
            print(imageName);
            GetComponent<Image>().sprite = Resources.Load<Sprite>(imageName);
            RectTransform buttonTransform = GetComponent<RectTransform>();
            print(buttonTransform.rect.y);
            float multiplier = 2*buttonTransform.rect.height/(GetComponent<Image>().sprite.bounds.extents.y);
            buttonTransform.sizeDelta = new Vector2(buttonTransform.sizeDelta.x /multiplier, buttonTransform.sizeDelta.y);
        }
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
