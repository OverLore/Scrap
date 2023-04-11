using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static ItemDB instance;

    public GameObject defaultBagPickable;

    Item[] ItemsDataBase = null;
    GameObject[] ItemsPrefabsDataBase = null;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        instance = this;

        ItemsDataBase = Resources.LoadAll<Item>("Items");

        ItemsPrefabsDataBase = Resources.LoadAll<GameObject>("Prefabs/Items");
    }

    public Item GetItemByID(string id)
    {
        if (ItemsDataBase.Length <= 0)
        {
            Debug.LogError("ItemDB est vide");

            return null;
        }

        foreach (Item item in ItemsDataBase)
        {
            if (item.id == id)
            {
                return Instantiate(item);
            }
        }

        return null;
    }

    public GameObject GetItemPrefabByID(string id)
    {
        if (ItemsPrefabsDataBase.Length <= 0)
        {
            Debug.LogError("ItemDB est vide");

            return null;
        }

        foreach (GameObject item in ItemsPrefabsDataBase)
        {
            Pickable p = item.GetComponent<Pickable>();

            if (p && p.item.id == id)
            {
                Item it = GetItemByID(p.item.id);
                
                return it.onFloorObject;
            }
        }

        return defaultBagPickable;
    }

    public Pickable DropItem(Item item, int quantity, Vector3 position, Vector3 force)
    {
        GameObject go = GetItemPrefabByID(item.id);

        if (!go)
            return null;

        go = Instantiate(go);
        go.transform.position = position;
        go.GetComponent<Rigidbody>().velocity = force;

        Pickable pickable = go.GetComponent<Pickable>();

        pickable.item = item;
        pickable.itemAmount = quantity;

        PickableManager.Instance.pickableList.Add(pickable);

        return pickable;
    }
}
