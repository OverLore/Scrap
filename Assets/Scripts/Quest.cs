using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Quest", fileName = "New Quest"), System.Serializable]
public class Quest : ScriptableObject
{
    [System.Serializable]
    public struct QuestData
    {
        public string id;
        public List<QuestItemData> currentItems;
    }
    
    [System.Serializable]
    public struct QuestItem
    {
        public Item item;
        public int amount;
    }
    
    [System.Serializable]
    public struct QuestItemData
    {
        public Item.ItemData item;
        public int amount;
    }

    [Header("General")]
    public string id;
    public string displayName;
    public string description;
    public Sprite icon;

    [Header("Requirement")]
    public List<QuestItem> requiredItems;

    [Header("Already done")]
    public List<QuestItem> currentItems;

    List<QuestItemData> CurrentQuestItemsToData()
    {
        List<QuestItemData> datas = new List<QuestItemData>();

        foreach (var item in currentItems)
        {
            QuestItemData data = new QuestItemData();

            data.item = item.item.CreateSaveData();
            data.amount = item.amount;

            datas.Add(data);
        }

        return datas;
    }

    void DataToCurrentQuestItems(List<QuestItemData> data)
    {
        currentItems = new List<QuestItem>();

        for (int i = 0; i < requiredItems.Count; i++)
        {
            QuestItem item = new QuestItem();

            item.amount = data[i].amount;
            item.item = Item.ReadSaveData(data[i].item);

            currentItems.Add(item);
        }
    }

    public QuestData CreateSaveData()
    {
        QuestData questData = new QuestData();

        questData.id = id;
        questData.currentItems = CurrentQuestItemsToData();

        return questData;
    }

    public static QuestData CreateSaveData(Quest _quest)
    {
        QuestData questData = new QuestData();

        questData.id = _quest == null ? "null" : _quest.id;
        questData.currentItems = _quest == null ? null : _quest.CurrentQuestItemsToData();

        return questData;
    }
    
    public static Quest ReadSaveData(QuestData _questData)
    {
        if (_questData.id == "null")
            return null;

        Quest quest = QuestDB.instance.GetQuestByID(_questData.id);
        quest.DataToCurrentQuestItems(_questData.currentItems);

        return quest;
    }
}
