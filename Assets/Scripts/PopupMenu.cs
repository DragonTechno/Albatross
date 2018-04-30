using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour {

    List<Quest> Quests;
    List<string> Items;
    Text QuestBoard;
    Text Inventory;
    CanvasGroup Menu;
    bool invis;

	// Use this for initialization
	void Start () {
        Quests = new List<Quest>();
        Items = new List<string>();
        Text[] menus = GetComponentsInChildren<Text>();
        Menu = GetComponent<CanvasGroup>();
        QuestBoard = menus[0];
        Inventory = menus[1];
        Menu.alpha = 0;
        Menu.interactable = false;
        Menu.blocksRaycasts = false;
        invis = true;
        UpdateMenus();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (invis)
            {
                Menu.alpha = 1;
                Menu.interactable = true;
                Menu.blocksRaycasts = true;
                invis = false;
            }
            else
            {
                Menu.alpha = 0;
                Menu.interactable = false;
                Menu.blocksRaycasts = false;
                invis = true;
            }
        }
	}

    void UpdateMenus ()
    {
        string qPrint = "Quests:" + '\n' + '\n';
        string iPrint = "Inventory:" + '\n' + '\n';
        foreach(Quest quest in Quests)
        {
            qPrint += "    " + quest.title + '\n';
        }
        foreach(string item in Items)
        {
            iPrint += "    " + item + '\n';
        }
        QuestBoard.text = qPrint;
        Inventory.text = iPrint;
    }

    public void AddQuest(Quest quest)
    {
        if (!Quests.Contains(quest))
        {
            Quests.Add(quest);
        }
        UpdateMenus();
    }

    public void RemoveQuest(Quest quest)
    {
        if (Quests.Contains(quest))
        {
            Quests.Remove(quest);
        }
        UpdateMenus();
    }

    public void AddQuest(string questName)
    {
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        quest.title = questName;
        if (!Quests.Contains(quest))
        {
            Quests.Add(quest);
        }
        UpdateMenus();
    }

    public void RemoveQuest(string questName)
    {
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        quest.title = questName;
        if (Quests.Contains(quest))
        {
            Quests.Remove(quest);
        }
        UpdateMenus();
    }

    public void AddItem(string item)
    {
        Items.Add(item);
        UpdateMenus();
    }

    public void RemoveItem(string item)
    {
        Items.Remove(item);
        UpdateMenus();
    }
}
