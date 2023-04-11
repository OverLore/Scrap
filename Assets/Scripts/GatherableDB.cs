using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherableDB : MonoBehaviour
{
    public static GatherableDB instance;

    GameObject[] GatherablesDataBase = null;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        instance = this;

        GatherablesDataBase = Resources.LoadAll<GameObject>("Gatherables");
    }

    public GameObject GetGatherableByID(string id)
    {
        if (GatherablesDataBase.Length <= 0)
        {
            Debug.LogError("GatherableDB est vide");

            return null;
        }

        foreach (GameObject gatherable in GatherablesDataBase)
        {
            if (gatherable.GetComponent<Gatherable>().id == id)
            {
                return gatherable;
            }
        }

        return null;
    }
}
