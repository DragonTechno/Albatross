using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// all objects with dialog and interaction on the isometric level
public class Dialog : Interactable
{
    public GameObject button;
    public bool scriptItem; // should be deleted at some point
    [TextArea(5, 100)]
    public string[] dialogBlock;
    internal List<Vector2> returnCoordinates; // save coords for previous dialog menu
    internal List<string> scriptText;
    internal int topOfPage = 0;
    internal int pageH = 0; // dialog block height of specific option menu
    internal int startingBlock = 0; // index of first block
    Vector3 textBottom; // bottom of dialog panel
    int pages; // total dialog block height, TODO change to pageH
    int cPage = -1; // c stands for current!
    int cBlock = 0;
    bool inDialog;
    bool active = true; // whether or not you can progress the block by clicking
    CanvasGroup dialogBox;
    string[] dialog; // all of the lines in the block you're currently in
    GameObject center; // centers the screen when entering dialog

    // Use this for initialization
    void Start()
    {
        menu = FindObjectOfType<PopupMenu>();
        dictionary = FindObjectOfType<GlobalDictionary>();
        returnCoordinates = new List<Vector2>();
        inDialog = false;
        thisSprite = GetComponent<SpriteRenderer>();
        Lock = FindObjectOfType<Canvas>().GetComponentInChildren<InteractionLock>();
        // easier to find the dialogbox from Lock
        dialogBox = Lock.gameObject.GetComponent<CanvasGroup>();
        dialog = dialogBlock[cBlock].Split('\n');
        pages = dialog.Length;
        float xPos = dialogBox.transform.position.x;
        float yPos = dialogBox.transform.position.y;
        // makes dialog window invisible when you start
        dialogBox.alpha = 0;
        dialogBox.interactable = false;
        dialogBox.blocksRaycasts = false;
        textBottom = new Vector3(xPos, yPos, 0);
        scriptText = new List<string>();
        originalColor = thisSprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        float xPos = dialogBox.transform.position.x;
        float yPos = dialogBox.transform.position.y - dialogBox.GetComponent<RectTransform>().rect.height / 2 * dialogBox.transform.parent.localScale.y;
        textBottom = new Vector3(xPos, yPos, 0);
        InteractionUpdate();
    }

    override public void InteractionAction()
    {
        if (active)
        {
            if (!inDialog)
            {
                //print("Dialog activation");
                inDialog = true;
                player.GetComponent<IsoMovement>().MovementToggle(false);
                dialogBox.alpha = 1;
                dialogBox.interactable = true;
                dialogBox.blocksRaycasts = true;
                center = new GameObject();
                float shiftUp = 2f; //These three lines create the object the camera hovers over during a conversation
                center.transform.position = transform.position - (transform.position - player.transform.position) / 2 + shiftUp * Vector3.up;
                FindObjectOfType<FollowObject>().SwitchFocus(center);
                InteractionAction();
            }
            else if (inDialog && cPage < pages - (pageH + 1))
            {
                RunString(scriptText); //Run any script associated with the last panel.
                scriptText = new List<string>();
                cPage += pageH;
                //print(pageH);
                pageH = 0;
                topOfPage = cPage + 1;
                //print("Parsing from dialog");
                string printLine = ParsePage();
                if(cPage <= dialog.Length)
                {
                    Text dialogText = dialogBox.GetComponentInChildren<Text>();
                    dialogText = dialogBox.GetComponentInChildren<Text>();
                    dialogText.text = printLine; //Add text to the panel
                    pageH += cPage - topOfPage;
                    cPage = topOfPage;
                }
                else
                {
                    InteractionAction();
                }
            }
            else
            {
                RunString(scriptText); //Run the script even if the window is closing.
                scriptText = new List<string>();
                if (cPage >= pages - (pageH + 1))
                {
                    pageH = 0;
                    if (returnCoordinates.Count == 0) //End the conversation if there is no return
                    {                                  //location.
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
                        SwitchBlocks((int)coordinate.x, (int)coordinate.y);
                        InteractionAction();
                    }
                }
                else
                {
                    pageH = 0;
                    InteractionAction();
                }
            }
        }
    }

    public string ParsePage()
    {
        ++cPage;
        //print(cPage);
        dialog = dialogBlock[cBlock].Split('\n');
        pages = dialog.Length;
        string startLine = dialog[cPage];
        //print("Start line " + startLine);
        string returnLine = "";
        string checkLine = "";
        //Check for an if block
        bool ifChecked = false;
        int timesAround = 0;
        while(!ifChecked)
        {
            ++timesAround;
            if(timesAround > 30)
            {
                return "Wowzers!";
            }
            ifChecked = true;
            if (startLine.Substring(0, 2) == "if")
            {
                ifChecked = false;
                bool booleanVal = CheckTruth(startLine);
                ++cPage;
                if (booleanVal)
                {
                    ++cPage;
                    startLine = dialog[cPage];
                }
                else
                {
                    int expectedElses = 1;
                    int seenElses = 0;
                    checkLine = dialog[cPage + 1];
                    string[] phrases = checkLine.Split(' ');

                    if (phrases[0] == "else")
                    {
                        seenElses += 1;
                    }
                    else if (phrases[0] == "if")
                    {
                        expectedElses += 1;
                    }
                    while (seenElses < expectedElses)
                    {
                        ++cPage;
                        checkLine = dialog[cPage + 1];
                        phrases = checkLine.Split(' ');
                        if (phrases[0] == "else")
                        {
                            seenElses += 1;
                        }
                        else if (phrases[0] == "if")
                        {
                            expectedElses += 1;
                        }
                    }
                    cPage += 2;
                    startLine = dialog[cPage];
                }
            }
            else if (startLine == "else")
            {
                ifChecked = false;
                print("Else");
                checkLine = dialog[cPage + 1];
                string[] phrases = checkLine.Split(' ');

                int expectedEnds = 1;
                int seenEnds = 0;
                if (phrases[0] == "end")
                {
                    seenEnds += 1;
                }
                else if (phrases[0] == "if")
                {
                    expectedEnds += 1;
                }
                while (seenEnds < expectedEnds)
                {
                    ++cPage;
                    checkLine = dialog[cPage + 1];
                    phrases = checkLine.Split(' ');
                    if (phrases[0] == "end")
                    {
                        seenEnds += 1;
                    }
                    else if (phrases[0] == "if")
                    {
                        expectedEnds += 1;
                    }
                }
                cPage += 2;
                if (cPage < dialog.Length)
                {
                    startLine = dialog[cPage];
                }
                else
                {
                    ifChecked = true;
                    cPage += 5;
                }
                //print("Line after end " + startLine);
            }
            else if (startLine == "end")
            {
                ifChecked = false;
                if (cPage < pages - 1)
                {
                    ++cPage;
                    if (cPage < dialog.Length)
                    {
                        startLine = dialog[cPage];
                    }
                    else
                    {
                        ifChecked = true;
                        cPage += 5;
                    }
                }
            }
        }
        if (startLine[0] == '\\')
        {
            Deactivate();
            topOfPage = cPage;
            int optsGenerated = 1;
            returnLine = startLine.Substring(1, startLine.Length - 1);
            checkLine = CheckAhead(dialog, cPage);
            while (checkLine[0] == '>' && cPage < pages - 1)
            {
                //print("Parsing from options");
                string buttonText = ParsePage();
                GameObject optionButton = Instantiate(button, textBottom, Quaternion.identity, dialogBox.transform);
                optionButton.transform.position = textBottom + (optsGenerated * .75f) * Vector3.up;
                optionButton.GetComponent<DialogButton>().Initialize(scriptText, this, buttonText);
                scriptText = new List<string>();
                //print(cPage);
                checkLine = CheckAhead(dialog, cPage);
                //print("Check line " + checkLine);
                ++optsGenerated;
            }
        }
        else if (startLine[0] == '>') //Check for an option
        {
            returnLine = startLine.Substring(2, startLine.Length - 2);
            //print(returnLine);
        }
        else //Otherwise, return normal text
        {
            returnLine = startLine;
        }
        //Check for scripts attached to a line.
        if (cPage < pages - 1)
        {
            checkLine = CheckAhead(dialog, cPage);
        }
        while (cPage < pages - 1 && checkLine[0] == '%')
        {
            //print("Script check " +checkLine);
            scriptText.Add(checkLine.Substring(2, checkLine.Length - 2));
            ++cPage;
            if (cPage < pages - 1)
            {
                checkLine = CheckAhead(dialog, cPage);
            }
            else
            {
                checkLine = "";
            }
        }
        return returnLine;
    }

    string CheckAhead(string[] cDialog,int currPage)
    {
        //print("Page in check ahead " + currPage);
        bool conditions = true;//Conditions represents whether or not we are, logically, inside of an if
        int timesAround = 0;
        while (conditions && currPage < cDialog.Length - 1)
        {
            ++timesAround;
            if (timesAround > 30)
            {
                return "Wowzers! 2!!!";
            }
            ++currPage;
            //print("Loop " + cDialog[currPage]);
            string[] phrases = cDialog[currPage].Split(' ');
            if (phrases[0] == "if")
            {
                bool checkBool = CheckTruth(cDialog[currPage]);
                //print(checkBool);
                if (checkBool)
                {
                    ++currPage;
                    //print("Checkbool " + cDialog[currPage]);
                }
                else
                {
                    int expectedElses = 1;
                    int seenElses = 0;
                    while (seenElses < expectedElses)
                    {
                        ++currPage;
                        phrases = cDialog[currPage].Split(' ');
                        if (phrases[0] == "else")
                        {
                            seenElses += 1;
                        }
                        else if (phrases[0] == "if")
                        {
                            expectedElses += 1;
                        }
                    }
                }
            }
            else if (phrases[0] == "else")
            {
                int expectedEnds = 1;
                int seenEnds = 0;
                while (seenEnds < expectedEnds)
                {
                    ++currPage;
                    phrases = cDialog[currPage].Split(' ');
                    if (phrases[0] == "end")
                    {
                        seenEnds += 1;
                    }
                    else if (phrases[0] == "if")
                    {
                        expectedEnds += 1;
                    }
                }
            }
            else if (phrases[0] == "end")
            {
                ++currPage;
                conditions = false;
            }
            else
            {
                conditions = false;
            }
        }
        if(currPage == cDialog.Length - 1)
        {
            if(cDialog[currPage] == "end")
            {
                ++currPage;
            }
        }
        //print(currPage + " " + pages + " " + cDialog.Length + "???");
        //print(cDialog[currPage] + " found ahead on page " + currPage + " with pages = " + pages);
        cPage = currPage - 1;
        if (currPage >= cDialog.Length)
        {
            --currPage;
            ++cPage;
        }
        return cDialog[currPage];
    }

    public void SwitchBlocks(int Block, int Page) //Switch to a line, without a return
    {
        cBlock = Block;
        dialog = dialogBlock[Block].Split('\n');
        pages = dialog.Length;
        cPage = Page - 1;
    }

    public void ReturnSwitch(int Block, int Page) //Switch to a line, to return later
    {
        returnCoordinates.Add(new Vector2(cBlock, cPage));
        cBlock = Block;
        dialog = dialogBlock[Block].Split('\n');
        pages = dialog.Length;
        cPage = Page - 1;
        //print(cBlock.ToString() + " " + cPage);
    }

    public void Deactivate() //Stop clicking the panel from progressing text
    {
        active = false;
    }

    public void Activate() //Make clicking the panel progress text.
    {
        active = true;
        scriptText = new List<string>();
        InteractionAction();
    }

    new public void RunString(List<string> scripts) //Run dialog scripts based on their names
    {
        foreach (string script in scripts)
        {
            //print(script);
            string tempScript = script.Replace("\n", string.Empty);
            string[] phrases = tempScript.Split(' ');
            if (phrases[0] == "goto")
            {
                SetLocation(1, 2, phrases,false);
            }
            else if (phrases[0] == "return")
            {
                SetLocation(1, 2, phrases,true);
            }
            else if (phrases[0] == "newpath")
            {
                returnCoordinates = new List<Vector2>();
            }
            else if (phrases[0] == "AddItem")
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
            }
            else if (phrases[0] == "RemQuest")
            {
                menu.RemoveQuest(phrases[1]);
                dictionary.BooleanDictionary[phrases[1] + "C"] = true;
            }
            else if (phrases[0] == "Continue")
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

    void SetLocation(int b, int p, string[] phrases, bool willReturn)
    {
        pageH = 0;
        int Block;
        int Page;
        int.TryParse(phrases[b], out Block);
        int.TryParse(phrases[p], out Page);
        if (willReturn)
        {
            ReturnSwitch(Block, Page);
        }
        else
        {
            SwitchBlocks(Block, Page);
        }
    }
}
