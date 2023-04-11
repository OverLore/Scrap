using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [Header("References")]
    public PanelInventory inventory;
    public PanelCursor cursor;
    public PanelHotbar hotbar;
    public PanelStorage storage;

    [SerializeField] private Canvas canvas;

    public bool isOpen;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        isOpen = true;

        inventory.Show();

        canvas.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        isOpen = false;

        inventory.Hide();
        storage.Hide();

        canvas.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();

        return isOpen;
    }
}
