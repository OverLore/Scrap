using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelCursor : InventoryPanel
{
    [Header("References")]
    [SerializeField] Slot slot;
    [SerializeField] PlayerController owner;

    private float m_StackAllDelay;
    private float m_MaxStackAllDelay = .3f;
    private Slot m_StackDelaySlot;

    private void Awake()
    {
        slot.onSetItem += OnItemChanged;
    }

    private void Start()
    {
        slot.SetItem(null);
    }
    
    public void HandleStackDelay(Slot _slot)
    {
        if (m_StackAllDelay > 0 && m_StackDelaySlot == _slot && _slot.GetItem() == null)
        {
            _slot.StartCollectStack();

            return;
        }
        
        m_StackAllDelay = m_MaxStackAllDelay;
        m_StackDelaySlot = _slot;
    }
    
    private void UpdateStackDelay()
    {
        if (m_StackAllDelay >= 0)
            m_StackAllDelay -= Time.deltaTime;
        else
            m_StackDelaySlot = null;
    }

    private void Update()
    {
        UpdateStackDelay();

        if (!isShown)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Mouse.current.position.ReadValue(), parentCanvas.worldCamera, out pos);
        transform.position = parentCanvas.transform.TransformPoint(pos);

        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Drop.triggered && !slot.Empty)
        {
            ItemDB.instance.DropItem(slot.GetItem(), slot.GetItem().currentStackAmount, owner.DropOrigin, owner.LookDir);

            slot.SetItem(null);
        }
    }

    private void OnItemChanged(Slot _slot)
    {
        if (slot.GetItem())
            Show();
        else
            Hide();
    }

    public Item GetItem()
    {
        return slot.GetItem();
    }

    public Slot GetSlot()
    {
        return slot;
    }
}
