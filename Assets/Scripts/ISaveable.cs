using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public interface ISaveable<T>
{
    public T CreateSaveData();
    public void ReadSaveData(T _data);
    public void Save(string _filePath)
    {
        T data;

        try
        {
            data = CreateSaveData();
        }
        catch (Exception e)
        {
            Debug.LogError("Exception catched (CreateSaveData) : " + e.Message + e.StackTrace);
            return;
        }


        string jsonString = JsonUtility.ToJson(data, true);

        if (!Directory.Exists(SaveManager.instance.saveFolderPath))
        {
            Directory.CreateDirectory(SaveManager.instance.saveFolderPath);
        }

        File.WriteAllText(_filePath, jsonString);
    }

    public void Load(string _filePath)
    {
        if (!Directory.Exists(SaveManager.instance.saveFolderPath))
            return;

        if (!File.Exists(_filePath))
            return;

        string json = File.ReadAllText(_filePath);

        T data = JsonUtility.FromJson<T>(json);

        try
        {
            ReadSaveData(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception catched (RetrieveFromSaveData) : " + e.Message + e.StackTrace);
        }
    }

    public string GetFileName();
}
