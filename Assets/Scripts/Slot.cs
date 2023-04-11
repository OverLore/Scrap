using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [System.Serializable]
    public struct SlotData
    {
        public Item.ItemData item;
    }

    public bool Empty { get { return (m_item == null || m_item.currentStackAmount <= 0); } }

    [Header("References")] 
    [SerializeField] Image iconImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image durabilityImage;
    [SerializeField] TMP_Text amountText;

    [Space(10), Header("Settings")]
    [SerializeField] Color normalColor;
    [SerializeField] Color hoverredColor;
    [SerializeField] Color durabilityImageColor;

    [Space(10), Header("DEBUG")]
    [SerializeField] Item DEBUG_Item;
    [SerializeField] int DEBUG_Amount;

    public Action<Slot> onSetItem;

    private Item m_item;
    private bool m_willStackAll;

    private void Start()
    {
        backgroundImage.color = normalColor;

        if (DEBUG_Item != null && !ManagersManager.instance.willLoad)
        {
            Item item = Instantiate(DEBUG_Item);
            item.currentStackAmount = Mathf.Min(DEBUG_Amount, item.maxStackAmount);

            SetItem(item);

            return;
        }

        Refresh();
    }

    public void SetItem(Item _item)
    {
        m_item = _item == null || _item.currentStackAmount <= 0 ? null : Instantiate(_item);

        Refresh();

        onSetItem?.Invoke(this);
    }

    public void Refresh()
    {
        if (m_item == null || m_item.currentStackAmount <= 0)
        {
            m_item = null;

            iconImage.sprite = null;
            iconImage.color = Color.clear;
            durabilityImage.color = Color.clear;
            amountText.text = String.Empty;
            amountText.color = new Color(0, 0, 0, 0);

            return;
        }

        iconImage.sprite = m_item.icon;
        iconImage.color = Color.white;

        durabilityImage.color = m_item.maxDurability == 0 ? Color.clear : durabilityImageColor;

        amountText.text = m_item.maxStackAmount == 1 ? String.Empty : m_item.currentStackAmount.ToString();
        amountText.color = m_item.maxStackAmount == 1 ? Color.clear : Color.white;
    }

    public void OnPointerEnter()
    {
        backgroundImage.color = hoverredColor;
    }

    public void OnPointerExit()
    {
        backgroundImage.color = normalColor;
    }

    public void OnPointerClick()
    {
        Item cursorItem = InventoryController.Instance.cursor.GetItem();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            InventoryController.Instance.cursor.HandleStackDelay(this);

            if (m_item == null)
            {
                SwapSlots(InventoryController.Instance.cursor.GetSlot());

                CollectStack();

                return;
            }

            if (InventoryController.Instance.cursor.GetItem() == null)
            {
                SwapSlots(InventoryController.Instance.cursor.GetSlot());
            }
            else
            {
                if (HasSameItem(InventoryController.Instance.cursor.GetSlot()))
                {
                    int toAdd = Mathf.Min(m_item.maxStackAmount - m_item.currentStackAmount, cursorItem.currentStackAmount);

                    if (toAdd == 0)
                    {
                        SwapSlots(InventoryController.Instance.cursor.GetSlot());

                        return;
                    }

                    m_item.currentStackAmount += toAdd;
                    cursorItem.currentStackAmount -= toAdd;

                    InventoryController.Instance.cursor.GetSlot().Refresh();
                    Refresh();

                    onSetItem?.Invoke(this);
                    InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());
                }
                else
                {
                    SwapSlots(InventoryController.Instance.cursor.GetSlot());
                }
            }

            return;
        }

        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.StackOne.WasReleasedThisFrame())
        {
            if (m_item == null)
            {
                if (cursorItem == null)
                    return;

                SetItem(cursorItem);
                m_item.currentStackAmount = 1;
                cursorItem.currentStackAmount--;

                InventoryController.Instance.cursor.GetSlot().Refresh();
                Refresh();

                onSetItem?.Invoke(this);
                InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());

                return;
            }

            if (cursorItem == null)
            {
                InventoryController.Instance.cursor.GetSlot().SetItem(m_item);
                InventoryController.Instance.cursor.GetItem().currentStackAmount = 1;
                InventoryController.Instance.cursor.GetSlot().Refresh();
                m_item.currentStackAmount--;

                InventoryController.Instance.cursor.GetSlot().Refresh();
                Refresh();

                onSetItem?.Invoke(this);
                InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());

                return;
            }

            if (!HasSameItem(InventoryController.Instance.cursor.GetSlot()) || m_item.currentStackAmount >= m_item.maxStackAmount)
                return;

            m_item.currentStackAmount++;
            cursorItem.currentStackAmount--;

            InventoryController.Instance.cursor.GetSlot().Refresh();
            Refresh();
            
            onSetItem?.Invoke(this);
            InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());

            return;
        }

        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.StackHalf.WasReleasedThisFrame())
        {
            int amount;

            if (m_item == null)
            {
                if (cursorItem == null)
                    return;

                amount = cursorItem.currentStackAmount / 2;

                SetItem(cursorItem);
                m_item.currentStackAmount = amount;
                cursorItem.currentStackAmount -= amount;
                
                InventoryController.Instance.cursor.GetSlot().Refresh();
                Refresh();

                onSetItem?.Invoke(this);
                InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());

                return;
            }

            if (cursorItem == null)
            {
                amount = m_item.currentStackAmount / 2;

                InventoryController.Instance.cursor.GetSlot().SetItem(m_item);
                InventoryController.Instance.cursor.GetItem().currentStackAmount = amount;
                InventoryController.Instance.cursor.GetSlot().Refresh();
                m_item.currentStackAmount -= amount;

                InventoryController.Instance.cursor.GetSlot().Refresh();
                Refresh();
                
                onSetItem?.Invoke(this);
                InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());

                return;
            }

            if (!HasSameItem(InventoryController.Instance.cursor.GetSlot()) || m_item.currentStackAmount >= m_item.maxStackAmount)
                return;

            amount = Mathf.Min(Mathf.Max(cursorItem.currentStackAmount / 2, 1), m_item.maxStackAmount - m_item.currentStackAmount);

            m_item.currentStackAmount += amount;
            cursorItem.currentStackAmount -= amount;

            InventoryController.Instance.cursor.GetSlot().Refresh();
            Refresh();
            
            onSetItem?.Invoke(this);
            InventoryController.Instance.cursor.GetSlot().onSetItem?.Invoke(InventoryController.Instance.cursor.GetSlot());

            return;
        }
    }

    public void StartCollectStack()
    {
        m_willStackAll = true;
    }

    public void CollectStack()
    {
        if (m_willStackAll)
        {
            InventoryController.Instance.inventory.StackAllInSlot(this);
            SwapSlots(InventoryController.Instance.cursor.GetSlot());
        }

        m_willStackAll = false;
    }

    public void SwapSlots(Slot _to)
    {
        Item temp = GetItem();
        SetItem(_to.GetItem());
        _to.SetItem(temp);
    }

    public Item GetItem()
    {
        return m_item;
    }

    public bool HasSameItem(Slot _slot)
    {
        return m_item != null && _slot.GetItem() != null && (m_item.id == _slot.GetItem().id);
    }

    public Vector3 GetPosition()
    {
        return GetComponent<Image>().rectTransform.anchoredPosition;
    }

    public SlotData CreateSaveData()
    {
        SlotData data = new SlotData();

        if (!Empty)
        {
            data.item = m_item.CreateSaveData();
        }
        else
        {
            data.item.id = "";
        }

        return data;
    }

    public void ReadSaveData(SlotData _data)
    {
        SetItem(null);

        if (_data.item.id != "")
        {
            Item item = Item.ReadSaveData(_data.item);

            SetItem(item);
        }
    }
}
