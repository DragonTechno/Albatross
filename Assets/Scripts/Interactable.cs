using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public GameObject player;
    public GameObject button;
    public float nearDistance;
    public bool scriptItem;
    [TextArea(5, 100)]
    public string[] dialogBlock;
    internal List<Vector2> returnCoordinates;
    internal string scriptText = "Identity";
    internal int buttonH = 0;
    internal int startingBlock = 0;
    Vector3 textBottom;
    int pages;
    int cPage = -1;
    int cBlock = 0;
    bool inDialog;
    bool active = true;
    InteractionLock Lock;
    CanvasGroup dialogBox;
    string[] dialog;
    SpriteRenderer thisSprite;
    GameObject center;

    // Use this for initialization
    void Start()
    {
        returnCoordinates = new List<Vector2>();
        inDialog = false;
        thisSprite = GetComponent<SpriteRenderer>();
        Lock = FindObjectOfType<Canvas>().GetComponentInChildren<InteractionLock>();
        dialogBox = Lock.gameObject.GetComponent<CanvasGroup>();
        dialog = dialogBlock[cBlock].Split('\n');
        pages = dialog.Length;
        float xPos = dialogBox.transform.position.x;
        float yPos = dialogBox.transform.position.y;
        dialogBox.alpha = 0;
        dialogBox.interactable = false;
        dialogBox.blocksRaycasts = false;
        textBottom = new Vector3(xPos, yPos, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float xPos = dialogBox.transform.position.x;
        float yPos = dialogBox.transform.position.y - dialogBox.GetComponent<RectTransform>().rect.height / 2 * dialogBox.transform.parent.localScale.y;
        textBottom = new Vector3(xPos, yPos, 0);
        if (Vector2.Distance(transform.position, player.transform.position) < nearDistance)
        {
            thisSprite.color = Color.yellow;
            if (Input.GetKeyDown(KeyCode.E))
            {
                ProgressText();
            }
        }
        else
        {
            thisSprite.color = Color.white;
        }
    }

    public void ProgressText()
    {
        if (active)
        {
            if (!inDialog)
            {
                if (!Lock.locked) //If the panel isn't in a conversation, start one.
                {
                    Lock.SetLock(this);
                    inDialog = true;
                    player.GetComponent<IsoMovement>().MovementToggle(false);
                    dialogBox.alpha = 1;
                    dialogBox.interactable = true;
                    dialogBox.blocksRaycasts = true;
                    center = new GameObject();
                    float shiftUp = 2f; //These three lines create the object the camera hovers over during a conversation
                    center.transform.position = transform.position - (transform.position - player.transform.position) / 2 + shiftUp * Vector3.up;
                    FindObjectOfType<FollowObject>().SwitchFocus(center);
                    ProgressText();
                }
            }
            else if (inDialog && cPage < pages - (1 + buttonH))
            {
                RunString(scriptText); //Run any script associated with the last panel.
                scriptText = "Identity"; //"Identity" just means "Continue", will probably change it to that.
                cPage += 1 + buttonH;
                Text dialogText = dialogBox.GetComponentInChildren<Text>();
                string printLine = dialog[cPage];
                string nextLine = "";
                int optsGenerated = 1;
                if (printLine[0] == '\\') //Check if we're at the beginning of an options block
                {
                    printLine = printLine.Substring(1, printLine.Length - 1);
                    ++buttonH;
                    if (cPage < pages - 1)
                    {
                        nextLine = dialog[cPage + 1];
                        if (nextLine[0] == '>')
                        {
                            ++cPage;
                        }
                        while (nextLine[0] == '>') //Parse through options
                        {
                            ++buttonH;
                            string buttonText = nextLine.Substring(2, nextLine.Length - 2);
                            if (cPage < pages - 1)
                            {
                                ++cPage;
                                nextLine = dialog[cPage];
                                string[] phrases = nextLine.Split(' ');
                                if (phrases[0] == "%") //Check for scripts
                                {
                                    scriptText = nextLine.Substring(2, nextLine.Length - 2);
                                    ++buttonH;
                                    if (cPage < pages - 1)
                                    {
                                        ++cPage;
                                        nextLine = dialog[cPage];
                                    }
                                }
                            }
                            else
                            {
                                nextLine = "";
                            }
                            GameObject optionButton = Instantiate(button, textBottom, Quaternion.identity, dialogBox.transform);
                            optionButton.transform.position = textBottom + (optsGenerated * .75f) * Vector3.up;
                            optionButton.GetComponent<DialogButton>().Initialize(scriptText, this, buttonText); scriptText = "Identity";
                            scriptText = "Identity";
                            ++optsGenerated;
                            Deactivate();
                        }
                    }
                    cPage -= buttonH + 1;
                }
                else
                {
                    if (cPage < pages - 1)
                    {
                        string checkLine = dialog[cPage + 1];
                        string[] phrases = checkLine.Split(' ');
                        if (phrases[0] == "%")//Check for scripts
                        {
                            scriptText = checkLine.Substring(2, checkLine.Length - 2);
                            cPage += 1;
                        }
                    }
                }
                dialogText = dialogBox.GetComponentInChildren<Text>();
                dialogText.text = printLine; //Add text to the panel
            }
            else
            {
                RunString(scriptText); //Run the script even if the window is closing.
                scriptText = "Identity";
                if (returnCoordinates.Count == 0) //End the conversation if there is no return
                {                                 //location.
                    inDialog = false;
                    Destroy(center); //Destroy the focus object from the conversation
                    FindObjectOfType<FollowObject>().SwitchFocus(player);
                    Lock.Unlock();
                    player.GetComponent<IsoMovement>().MovementToggle(true);
                    dialogBox.alpha = 0;
                    dialogBox.interactable = false;
                    dialogBox.blocksRaycasts = false;
                    cBlock = startingBlock;
                    cPage = -1;
                }
                else
                {   //If the return list isn't empty, go to its last location.
                    Vector2 coordinate = returnCoordinates[returnCoordinates.Count - 1];
                    returnCoordinates.RemoveAt(returnCoordinates.Count - 1);
                    SwitchBlocks((int)coordinate.x, (int)coordinate.y + 1);
                    ProgressText();
                }
            }
        }
    }

    public void SwitchBlocks(int Block, int Page) //Switch to a line, without a return
    {
        buttonH = 0;
        cBlock = Block;
        dialog = dialogBlock[Block].Split('\n');
        pages = dialog.Length;
        cPage = Page - 1;
    }

    public void ReturnSwitch(int Block, int Page) //Switch to a line, to return later
    {
        buttonH = 0;
        returnCoordinates.Add(new Vector2(cBlock, cPage));
        cBlock = Block;
        dialog = dialogBlock[Block].Split('\n');
        pages = dialog.Length;
        cPage = Page - 1;
    }

    public void Deactivate()
    {
        active = false;
    }

    public void Activate()
    {
        active = true;
        ProgressText();
    }

    public void RunString(string script) //Run dialog scripts based on their names
    {
        script = script.Replace("\n", string.Empty);
        string[] phrases = script.Split(' ');
        if (phrases[0] == "goto")
        {
            int Block;
            int Page;
            int.TryParse(phrases[1], out Block);
            int.TryParse(phrases[2], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            SwitchBlocks(Block, Page);
        }
        else if (phrases[0] == "return")
        {
            int Block;
            int Page;
            int.TryParse(phrases[1], out Block);
            int.TryParse(phrases[2], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            ReturnSwitch(Block, Page);
        }
        else if (phrases[0] == "newpath")
        {
            int Block;
            int Page;
            int.TryParse(phrases[1], out Block);
            int.TryParse(phrases[2], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            returnCoordinates = new List<Vector2>();
            SwitchBlocks(Block, Page);
        }
        else if (phrases[0] == "AddItem")
        {
            int Block;
            int Page;
            int.TryParse(phrases[2], out Block);
            int.TryParse(phrases[3], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            ReturnSwitch(Block, Page);
            FindObjectOfType<PopupMenu>().AddItem(phrases[1]);
        }
        else if (phrases[0] == "RemItem")
        {
            int Block;
            int Page;
            int.TryParse(phrases[2], out Block);
            int.TryParse(phrases[3], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            ReturnSwitch(Block, Page);
            FindObjectOfType<PopupMenu>().RemoveItem(phrases[1]);
        }
        else if (phrases[0] == "AddQuest")
        {
            int Block;
            int Page;
            int.TryParse(phrases[2], out Block);
            int.TryParse(phrases[3], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            ReturnSwitch(Block, Page);
            FindObjectOfType<PopupMenu>().AddQuest(phrases[1]);
        }
        else if (phrases[0] == "RemQuest")
        {
            int Block;
            int Page;
            int.TryParse(phrases[2], out Block);
            int.TryParse(phrases[3], out Page);
            print("Block " + Block.ToString());
            print("Page " + Page.ToString());
            ReturnSwitch(Block, Page);
            FindObjectOfType<PopupMenu>().RemoveQuest(phrases[1]);
        }
        else if (phrases[0] == "Identity")
        {
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
