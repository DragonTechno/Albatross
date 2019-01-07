using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour {

    internal GlobalDictionary dictionary;
    Text QuestBoard;
    Text Inventory;
    CanvasGroup Menu;
    bool invis;

	// Use this for initialization
	void Start () {
        dictionary = FindObjectOfType<GlobalDictionary>();
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
        foreach(Quest quest in dictionary.Quests)
        {
            qPrint += "    " + quest.title + '\n';
        }
        foreach(Item item in dictionary.Items)
        {
            iPrint += "    " + item.title + '\n';
        }
        QuestBoard.text = qPrint;
        Inventory.text = iPrint;
    }

    public void AddQuest(Quest quest)
    {
        if (!dictionary.Quests.Contains(quest))
        {
            dictionary.Quests.Add(quest);
        }
        UpdateMenus();
    }

    public void RemoveQuest(Quest quest)
    {
        if (dictionary.Quests.Contains(quest))
        {
            dictionary.Quests.Remove(quest);
        }
        UpdateMenus();
    }

    public void AddQuest(string questName)
    {
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        quest.title = questName;
        if (!dictionary.Quests.Contains(quest))
        {
            dictionary.Quests.Add(quest);
        }
        UpdateMenus();
    }

    public void RemoveQuest(string questName)
    {
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        quest.title = questName;
        if (dictionary.Quests.Contains(quest))
        {
            dictionary.Quests.Remove(quest);
        }
        UpdateMenus();
    }

    public void AddItem(string itemName)
    {
        Item item = ScriptableObject.CreateInstance<Item>();
        item.title = itemName;
        dictionary.Items.Add(item);
        UpdateMenus();
    }

    public void RemoveItem(string itemName)
    {
        Item item = ScriptableObject.CreateInstance<Item>();
        item.title = itemName;
        if(dictionary.Items.Contains(item))
        {
            dictionary.Items.Remove(item);
        }
        UpdateMenus();
    }
}
