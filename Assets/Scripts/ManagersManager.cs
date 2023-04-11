using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    public static ManagersManager instance;

    public bool willLoad;

    public InputManager inputManager;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Destroy(gameObject);

        inputManager = new InputManager();
        inputManager.Init();
    }
}
