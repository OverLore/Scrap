using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour, ISaveable<StorageManager.StorageManagerData>
{
    [System.Serializable]
    public struct StorageManagerData
    {
        public List<Storage.InventoryData> storagesList;
    }

    [HideInInspector] public List<Storage> storageList = new List<Storage>();

    private static StorageManager instance;
    public static StorageManager Instance { get { return instance; } }

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

        AddSceneStartStoragesToList();
    }

    public void AddSceneStartStoragesToList()
    {
        Storage[] startStorages = FindObjectsOfType<Storage>();

        if (!ManagersManager.instance.willLoad)
            foreach (var startStorage in startStorages)
                storageList.Add(startStorage);
        else
            foreach (var startStorage in startStorages)
                Destroy(startStorage.gameObject);
    }

    public StorageManagerData CreateSaveData()
    {
        StorageManagerData data = new StorageManagerData();

        data.storagesList = new List<Storage.InventoryData>();
        foreach (var storage in storageList)
        {
            Storage.InventoryData storageData = storage.CreateSaveData();
            data.storagesList.Add(storageData);
        }

        return data;
    }

    public void ReadSaveData(StorageManagerData _data)
    {
        for (int i = 0; i < _data.storagesList.Count; i++)
        {
            Storage storage = Instantiate(StorageDB.instance.GetStorageByID(_data.storagesList[i].id)).GetComponent<Storage>();
            storage.ReadSaveData(_data.storagesList[i]);

            storageList.Add(storage);
        }
    }

    public string GetFileName()
    {
        return typeof(StorageManager).ToString();
    }
}
