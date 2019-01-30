using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : Interactable
{
    public int storeWidth;
    public int storeHeight;
    Vector3 textBottom;
    public CanvasGroup dialogBox;
    public ItemHolder[] itemsInShop;
    public ShopItem[] itemsWithCosts;
    public GameObject[] itemButtons;

    public struct ShopItem
    {
        Item item;
        int cost;
    };

    // Start is called before the first frame update
    void Start()
    {
        menu = FindObjectOfType<PopupMenu>();
        dictionary = FindObjectOfType<GlobalDictionary>();
        thisSprite = GetComponent<SpriteRenderer>();
        Lock = FindObjectOfType<Canvas>().GetComponentInChildren<InteractionLock>();
        originalColor = thisSprite.color;
        float xPos = dialogBox.transform.position.x;
        float yPos = dialogBox.transform.position.y;
        // makes dialog window invisible when you start
        dialogBox.alpha = 0;
        dialogBox.interactable = false;
        dialogBox.blocksRaycasts = false;
        textBottom = new Vector3(xPos, yPos, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
