using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableManager : MonoBehaviour, ISaveable<PickableManager.PickableManagerData>
{
    [HideInInspector] public List<Pickable> pickableList = new List<Pickable>();

    [System.Serializable]
    public struct PickableManagerData
    {
        public List<Pickable.PickableData> pickablesList;
    }

    private static PickableManager instance;
    public static PickableManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SaveManager.instance.Register(this);

        AddSceneStartPickablesToList();
    }

    public void AddSceneStartPickablesToList()
    {
        Pickable[] startPickables = FindObjectsOfType<Pickable>();

        if (!ManagersManager.instance.willLoad)
            foreach (var startPickable in startPickables)
                pickableList.Add(startPickable);
        else
            foreach (var startPickable in startPickables)
                Destroy(startPickable.gameObject);
    }

    public PickableManagerData CreateSaveData()
    {
        PickableManagerData data = new PickableManagerData();

        data.pickablesList = new List<Pickable.PickableData>();
        foreach (var pickable in pickableList)
        {
            Pickable.PickableData pickableData = pickable.CreateSaveData();
            data.pickablesList.Add(pickableData);
        }

        return data;
    }

    public void ReadSaveData(PickableManagerData _data)
    {
        for (int i = 0; i < _data.pickablesList.Count; i++)
        {
            Item refItem = Item.ReadSaveData(_data.pickablesList[i].refItem);
            Pickable pickable = ItemDB.instance.DropItem(refItem, refItem.currentStackAmount, _data.pickablesList[i].position, Vector3.zero);
            pickable.ReadSaveData(_data.pickablesList[i]);
        }
    }

    public string GetFileName()
    {
        return typeof(PickableManager).ToString();
    }
}
