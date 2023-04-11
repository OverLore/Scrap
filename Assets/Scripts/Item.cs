using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Item", fileName = "New Item"), System.Serializable]
public class Item : ScriptableObject
{
    [System.Serializable]
    public struct ItemData
    {
        public string id;
        public int currentStackAmount;
        public float currentDurability;
    }

    [Header("General")] 
    public string id;
    public string displayName;
    public string description;
    public Sprite icon;
    public string data;
    public GameObject inHandObject;
    public GameObject onFloorObject;

    [Header("Stack")]
    public int maxStackAmount;
    public int currentStackAmount;

    [Header("Durability")]
    public float maxDurability;
    public float currentDurability;

    public ItemData CreateSaveData()
    {
        ItemData itemData = new ItemData();

        itemData.id = id;
        itemData.currentStackAmount = currentStackAmount;
        itemData.currentDurability = currentDurability;

        return itemData;
    }

    public static ItemData CreateSaveData(Item _item)
    {
        ItemData itemData = new ItemData();

        itemData.id = _item == null ? "null" : _item.id;
        itemData.currentStackAmount = _item == null ? 0 : _item.currentStackAmount;
        itemData.currentDurability = _item == null ? 0 : _item.currentDurability;

        return itemData;
    }

    public static Item ReadSaveData(ItemData _itemData)
    {
        if (_itemData.id == "null")
            return null;

        Item item = ItemDB.instance.GetItemByID(_itemData.id);
        
        item.currentStackAmount = _itemData.currentStackAmount;
        item.currentDurability = _itemData.currentDurability;

        return item;
    }
}
