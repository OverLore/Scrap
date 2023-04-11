using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, InitializeOnLoadAttribute]
public class SaveTools : MonoBehaviour
{
    public static bool IsRed => Directory.Exists(Path.Combine(Application.persistentDataPath, "Save")) && File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, "Save"), "State.txt"));

    static SaveTools()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !IsRed)
            return;

        if (UnityEditor.EditorUtility.DisplayDialog("Sauvegarde existante", "Une sauvegarde existe déjà, la supprimer ?", "Oui", "Non"))
        {
            string path = Path.Combine(Application.persistentDataPath, "Save");

            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }

    private void Update()
    {
        name = IsRed ? "EDITOR ONLY (HAS SAVE)" : "EDITOR ONLY (NO SAVE)";
        var iconContent = EditorGUIUtility.IconContent(IsRed ? "sv_label_6" : "sv_label_3");
        EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)iconContent.image);
    }

#if UNITY_EDITOR
    [MenuItem("Save/Open", false, 0)]
    static void OpenSaveFolder()
    {
        string path = Path.Combine(Application.persistentDataPath, "Save");
        string savePath = path.Replace(@"/", @"\");

        Process.Start("explorer.exe", $"/select,{savePath}");
    }


    [MenuItem("Save/Delete", false, 0)]
    static void DeleteSaves()
    {
        string path = Path.Combine(Application.persistentDataPath, "Save");

        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }


    [MenuItem("Save/Clear PlayerPrefs", false, 0)]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}
