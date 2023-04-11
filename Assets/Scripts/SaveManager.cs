using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [HideInInspector] public string saveFolderPath;

    Action OnSaveGame;
    Action OnLoadGame;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        saveFolderPath = Path.Combine(Application.persistentDataPath, "Save");

        ManagersManager.instance.willLoad = Directory.Exists(saveFolderPath) && File.Exists(Path.Combine(saveFolderPath, "State.txt"));
    }

    private void Start()
    {
        if (ManagersManager.instance.willLoad)
            Load();
        else
            StartCoroutine(SaveFirstGame());
    }

    public void Register<T>(ISaveable<T> saveable)
    {
        string filePath = Path.Combine(saveFolderPath, $"{GetFileName(saveable.GetFileName())}.txt");

        OnSaveGame += () => saveable.Save(filePath);
        OnLoadGame += () => saveable.Load(filePath);
    }

    public void Save()
    {
        OnSaveGame?.Invoke();

        File.WriteAllText(Path.Combine(saveFolderPath, "State.txt"), "0");
    }

    public void Load()
    {
        OnLoadGame?.Invoke();
    }

    string GetFileName(string _fileName)
    {
        string fileName = _fileName;
        string[] fileNameParts = fileName.Split("+");

        if (fileNameParts.Length > 1)
            fileName = fileName.Split("+")[1];

        return fileName;
    }

    public IEnumerator SaveFirstGame()
    {
        yield return new WaitForFixedUpdate();

        Save();
    }
}
