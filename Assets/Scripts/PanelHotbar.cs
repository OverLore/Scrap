using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHotbar : PanelInventory
{
    public Item EquippedItem { get { return m_slots[m_selectedSlotId].GetItem(); } }
    public Slot SelectedSlot { get { return m_slots[m_selectedSlotId]; } }

    [Header("References")]
    [SerializeField] Image selectionSquare;
    [SerializeField] Transform selectionSlotsGrid;
    [SerializeField] PlayerController player;
    
    private int m_selectedSlotId = 0;
    
    protected List<Image> m_selectionSlot = new List<Image>();

    private float nextAllowedSwitchIn = 0;
    private float switchDelay = 0.028f;

    private void LoadSlots()
    {
        if (!selectionSlotsGrid || selectionSlotsGrid.childCount <= 0)
            return;
        
        for (int i = 0; i < selectionSlotsGrid.childCount; i++)
            m_selectionSlot.Add(selectionSlotsGrid.GetChild(i).GetComponent<Image>());
    }

    public void SelectSlot(int id)
    {
        if (m_selectedSlotId == id)
            return;

        m_selectedSlotId = id;
        Refresh();

        UpdateSelection();
    }

    private void Awake()
    {
        base.Awake();


        
        LoadSlots();
        Refresh();

        foreach (var slot in m_slots)
        {
            slot.onSetItem += OnHotbarSlotHasChanged;
        }
    }

    private void Update()
    {
        if (nextAllowedSwitchIn > 0)
        {
            nextAllowedSwitchIn -= Time.deltaTime;
        }

        HandleInputs();
    }

    private void HandleInputs()
    {
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Drop.triggered && !SelectedSlot.Empty && !InventoryController.Instance.isOpen)
        {
            ItemDB.instance.DropItem(SelectedSlot.GetItem(), SelectedSlot.GetItem().currentStackAmount, player.DropOrigin, player.LookDir);

            SelectedSlot.SetItem(null);

            Refresh();
            UpdateSelection();
        }

        if (nextAllowedSwitchIn <= 0 && ManagersManager.instance.inputManager.Inputs.PlayerGround.NextSlot.ReadValue<float>() != 0)
        {
            nextAllowedSwitchIn = switchDelay;

            m_selectedSlotId += ManagersManager.instance.inputManager.Inputs.PlayerGround.NextSlot.ReadValue<float>() < 0 ? 1 : -1;

            if (m_selectedSlotId < 0)
            {
                m_selectedSlotId = m_slots.Count - 1;
            }
            else if (m_selectedSlotId > m_slots.Count - 1)
            {
                m_selectedSlotId = 0;
            }

            Refresh();
            UpdateSelection();

            return;
        }

        int fastSlot = 0;

        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Slot1.triggered)
            fastSlot = 1;
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Slot2.triggered)
            fastSlot = 2;
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Slot3.triggered)
            fastSlot = 3;
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Slot4.triggered)
            fastSlot = 4;
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Slot5.triggered)
            fastSlot = 5;
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Slot6.triggered)
            fastSlot = 6;

        if (fastSlot > 0)
        {
            m_selectedSlotId = fastSlot - 1;

            Refresh();
            UpdateSelection();
        }
    }

    private void OnHotbarSlotHasChanged(Slot _slot)
    {
        if (_slot == m_slots[m_selectedSlotId])
            UpdateSelection();
    }
    
    private void UpdateSelection()
    {
        player.ClearHand();

        if (EquippedItem != null)
            player.SetObjectInHand(EquippedItem);
    }

    private void Refresh()
    {
        for (int i = 0; i < m_selectionSlot.Count; i++)
        {
            if (i == m_selectedSlotId)
                m_selectionSlot[i].color = Color.white;
            else
                m_selectionSlot[i].color = Color.clear;
        }
    }

    public override string GetFileName()
    {
        return typeof(PanelHotbar).ToString();
    }
}
