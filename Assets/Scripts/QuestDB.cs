using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDB : MonoBehaviour
{
    public static QuestDB instance;

    Quest[] QuestsDataBase = null;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        instance = this;

        QuestsDataBase = Resources.LoadAll<Quest>("Quests");
    }

    public Quest GetQuestByID(string id)
    {
        if (QuestsDataBase.Length <= 0)
        {
            Debug.LogError("QuestDB est vide");

            return null;
        }

        foreach (Quest quest in QuestsDataBase)
        {
            if (quest.id == id)
            {
                return Instantiate(quest);
            }
        }

        return null;
    }
}
