using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageDB : MonoBehaviour
{
    public static StorageDB instance;

    GameObject[] StoragesDataBase = null;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        instance = this;

        StoragesDataBase = Resources.LoadAll<GameObject>("Storages");
    }

    public GameObject GetStorageByID(string id)
    {
        if (StoragesDataBase.Length <= 0)
        {
            Debug.LogError("StorageDB est vide");

            return null;
        }

        foreach (GameObject storage in StoragesDataBase)
        {
            if (storage.GetComponent<Storage>().id == id)
            {
                return storage;
            }
        }

        return null;
    }
}
