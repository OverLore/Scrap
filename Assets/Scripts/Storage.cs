using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Storage : MonoBehaviour, IInteractable, ISaveable<Storage.InventoryData>
{
    [System.Serializable]
    public struct InventoryData
    {
        public string displayName;
        public string id;
        public int amountOfSlots;
        public Vector3 position;
        public Vector3 rotation;
        public List<Item.ItemData> listItems;
    }

    [Header("Settings")]
    [SerializeField] private string displayName;
    [SerializeField] private int amountOfSlots = 12;
    [SerializeField] public string id;

    List<Item> content;
    List<Slot> slots;

    private void Awake()
    {
        InitContent();
    }

    void InitContent()
    {
        content = new List<Item>();

        for (int i = 0; i < amountOfSlots; i++)
            content.Add(null);
    }

    public void Open()
    {
        InventoryController ic = InventoryController.Instance;

        slots = new List<Slot>();

        for (int i = ic.storage.grid.childCount - 1; i >= 0; i--)
        {
            Destroy(ic.storage.grid.GetChild(i).gameObject);
        }

        for (int i = 0; i < amountOfSlots; i++)
        {
            GameObject go = Instantiate(ic.storage.slotPrefab, ic.storage.grid);
            Slot s = go.GetComponent<Slot>();

            s.onSetItem += OnSlotChanged;
            slots.Add(s);

            if (content[i] != null)
                s.SetItem(content[i]);
        }

        ic.Open();
        ic.storage.Show();
    }

    private void OnDestroy()
    {
        StorageManager.Instance.storageList.Remove(this);
    }

    public void OnSlotChanged(Slot _s)
    {
        for (int i = 0; i < slots.Count; i++)
            if (slots[i] == _s)
                content[i] = _s.GetItem();
    }

    public void DoInteraction()
    {
        Open();
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();
        ActionUI.Instance.SetText($"Press E to open {displayName}");
    }

    public InventoryData CreateSaveData()
    {
        InventoryData data = new InventoryData();

        data.amountOfSlots = amountOfSlots;
        data.displayName = displayName;
        data.position = transform.position;
        data.rotation = transform.rotation.eulerAngles;
        data.id = id;

        data.listItems = new List<Item.ItemData>();
        foreach (var item in content)
        {
            Item.ItemData slotData = Item.CreateSaveData(item);

            data.listItems.Add(slotData);
        }

        return data;
    }

    public void ReadSaveData(InventoryData _data)
    {
        amountOfSlots = _data.amountOfSlots;
        displayName = _data.displayName;
        transform.position = _data.position;
        transform.rotation = Quaternion.Euler(_data.rotation);
        id = _data.id;

        InitContent();

        for (int i = 0; i < content.Count; i++)
        {
            content[i] = Item.ReadSaveData(_data.listItems[i]);
        }
    }

    public string GetFileName()
    {
        return typeof(Storage).ToString();
    }
}
