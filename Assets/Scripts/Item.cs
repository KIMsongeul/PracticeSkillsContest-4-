using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemNum;
    GameManger gameManager;
    public string itemName;
    public int num;
    public int weight;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManger>();
    }
    public void SetItem(int itemNum)
    {
        this.itemNum = itemNum;
        itemName = GameData.sName[this.itemNum];
        num = 1;
        weight = GameData.iWeight[this.itemNum];
    }

}
