using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherableManager : MonoBehaviour, ISaveable<GatherableManager.GatherableManagerData>
{
    [System.Serializable]
    public struct GatherableManagerData
    {
        public List<Gatherable.GatherableData> gatherablesList;
    }

    [HideInInspector] public List<Gatherable> gatherableList = new List<Gatherable>();

    private static GatherableManager instance;
    public static GatherableManager Instance { get { return instance; } }

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

        AddSceneStartGatherablesToList();
    }

    public void AddSceneStartGatherablesToList()
    {
        Gatherable[] startGatherables = FindObjectsOfType<Gatherable>();

        if (!ManagersManager.instance.willLoad)
            foreach (var startGatherable in startGatherables)
                gatherableList.Add(startGatherable);
        else
            foreach (var startGatherable in startGatherables)
                Destroy(startGatherable.gameObject);
    }

    public GatherableManagerData CreateSaveData()
    {
        GatherableManagerData data = new GatherableManagerData();

        data.gatherablesList = new List<Gatherable.GatherableData>();
        foreach (var gatherable in gatherableList)
        {
            Gatherable.GatherableData gatherableData = gatherable.CreateSaveData();
            data.gatherablesList.Add(gatherableData);
        }

        return data;
    }

    public void ReadSaveData(GatherableManagerData _data)
    {
        for (int i = 0; i < _data.gatherablesList.Count; i++)
        {
            Gatherable gatherable = Instantiate(GatherableDB.instance.GetGatherableByID(_data.gatherablesList[i].id)).GetComponent<Gatherable>();
            gatherable.ReadSaveData(_data.gatherablesList[i]);

            gatherableList.Add(gatherable);
        }
    }

    public string GetFileName()
    {
        return typeof(GatherableManager).ToString();
    }
}
