using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gatherable : MonoBehaviour, ISaveable<Gatherable.GatherableData>
{
    [System.Serializable]
    public struct AvailableTool
    {
        public Item tool;
        public float efficiency;
    }

    [System.Serializable]
    public struct GatherableItem
    {
        public Item resource;
        public int chance;
        public int dropAmount;
    }

    [System.Serializable]
    public struct AvailableToolData
    {
        public Item.ItemData tool;
        public float efficiency;
    }

    [System.Serializable]
    public struct GatherableItemData
    {
        public Item.ItemData resource;
        public int chance;
        public int dropAmount;
    }

    [System.Serializable]
    public struct GatherableData
    {
        public int availableToolsAmount;
        public List<AvailableToolData> availableTools;
        public int itemPoolAmount;
        public List<GatherableItemData> itemPool;
        public int life;
        public Vector3 position;
        public Vector3 rotation;
        public string id;
    }

    [Header("Settings")]
    [SerializeField] public string id;

    [Header("Tools")]
    [SerializeField] List<AvailableTool> availableTools = new List<AvailableTool>();

    [Header("Gathering")]
    [SerializeField] int life;
    [SerializeField] List<GatherableItem> itemPool = new List<GatherableItem>();

    [Header("FX")]
    public GameObject breakFX;

    public virtual Item Gather(Item tool)
    {
        if (tool == null)
            return null;

        AvailableTool availableTool = availableTools.Find(x => x.tool.id == tool.id);

        if (availableTool.tool == null)
            return null;

        int totalChance = 0;
        foreach (GatherableItem item in itemPool)
            totalChance += item.chance;

        int random = Random.Range(0, totalChance);

        Item it = null;

        int currentChance = 0;
        foreach (GatherableItem item in itemPool)
        {
            currentChance += item.chance;

            if (random <= currentChance)
            {
                int amount = (int)(availableTool.efficiency * item.dropAmount);
                if (amount == 0)
                    amount = 1;
                it = Instantiate(item.resource);
                it.currentStackAmount = amount;
                
                Item rest = InventoryController.Instance.inventory.AddItem(it);
                if (rest != null)
                    Debug.LogError("TODO : Drop rest item");
                
                break;
            }
        }

        life--;

        if (life <= 0)
            Break();

        return it;
    }

    protected void Break()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GatherableManager.Instance.gatherableList.Remove(this);
    }

    List<AvailableToolData> AvailableToolsToData()
    {
        List<AvailableToolData> datas = new List<AvailableToolData>();

        foreach (var tool in availableTools)
        {
            AvailableToolData data = new AvailableToolData();

            data.efficiency = tool.efficiency;
            data.tool = tool.tool.CreateSaveData();

            datas.Add(data);
        }

        return datas;
    }

    void DataToAvailableTools(List<AvailableToolData> data, int amount)
    {
        availableTools = new List<AvailableTool>();

        for (int i = 0; i < amount; i++)
        {
            AvailableTool tool = new AvailableTool();

            tool.efficiency = data[i].efficiency;
            tool.tool = Item.ReadSaveData(data[i].tool);

            availableTools.Add(tool);
        }
    }

    List<GatherableItemData> GatherableItemsToData()
    {
        List<GatherableItemData> datas = new List<GatherableItemData>();

        foreach (var item in itemPool)
        {
            GatherableItemData data = new GatherableItemData();

            data.chance = item.chance;
            data.dropAmount = item.dropAmount;
            data.resource = item.resource.CreateSaveData();

            datas.Add(data);
        }

        return datas;
    }

    void DataToGatherableItems(List<GatherableItemData> data, int amount)
    {
        itemPool = new List<GatherableItem>();

        for (int i = 0; i < amount; i++)
        {
            GatherableItem item = new GatherableItem();

            item.chance = data[i].chance;
            item.dropAmount = data[i].dropAmount;
            item.resource = Item.ReadSaveData(data[i].resource);

            itemPool.Add(item);
        }
    }

    public GatherableData CreateSaveData()
    {
        GatherableData data = new GatherableData();

        data.availableTools = AvailableToolsToData();
        data.availableToolsAmount = availableTools.Count;
        data.itemPool = GatherableItemsToData();
        data.itemPoolAmount = itemPool.Count;
        data.life = life;
        data.position = transform.position;
        data.rotation = transform.rotation.eulerAngles;
        data.id = id;

        return data;
    }

    public void ReadSaveData(GatherableData _data)
    {
        DataToAvailableTools(_data.availableTools, _data.availableToolsAmount);
        DataToGatherableItems(_data.itemPool, _data.itemPoolAmount);
        life = _data.life;
        id = _data.id;
        transform.position = _data.position;
        transform.rotation = Quaternion.Euler(_data.rotation);
    }

    public string GetFileName()
    {
         return typeof(Gatherable).ToString();
    }
}
