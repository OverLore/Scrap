using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Slot : MonoBehaviour
{
    public bool IsEmpty => item == null || item.currentAmount < 1;
    public bool IsFull => item != null && item.currentAmount >= item.maxStackSize;

    [Header("Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image amountImage;
    [SerializeField] private Image durabilityBarImage;
    [SerializeField] private Image durabilityBarFillImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Animator animator;

    [Space(10), Header("Colors")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color durabilityMaxColor;
    [SerializeField] private Color durabilityMinColor;

    public Action OnItemChange;

    private bool m_hoverred;
    private bool clicked;

    [SerializeField] private Inventory owner;

    private Item item;

    private void Awake()
    {
        Refresh();
    }

    private void Update()
    {
        if (m_hoverred && 
            (Mouse.current.leftButton.wasPressedThisFrame ||
             Mouse.current.rightButton.wasPressedThisFrame ||
             Mouse.current.middleButton.wasPressedThisFrame))
        {
            clicked = true;
        }

        if (m_hoverred && clicked &&
            (Mouse.current.leftButton.wasReleasedThisFrame ||
             Mouse.current.rightButton.wasReleasedThisFrame ||
             Mouse.current.middleButton.wasReleasedThisFrame))
        {
            MouseClickEvent();
        }
    }

    private void RefreshColor()
    {
        backgroundImage.color = m_hoverred ? hoverColor : normalColor;
    }

    public void PlayTriggerPop()
    {
        animator.SetTrigger("EnterPop");
    }

    public void MouseEnterEvent()
    {
        m_hoverred = true;

        PlayTriggerPop();

        RefreshColor();
    }

    public void MouseLeaveEvent()
    {
        m_hoverred = false;
        clicked = false;

        RefreshColor();
    }

    public void SetOwner(Inventory _inventory)
    {
        owner = _inventory;
    }

    public Inventory GetOwner()
    {
        return owner;
    }

    public Item GetItem()
    {
        return item;
    }

    public void Refresh()
    {
        amountText.text = string.Empty;
        itemImage.sprite = null;
        itemImage.SetAlpha(0);
        durabilityBarImage.SetAlpha(0);
        durabilityBarFillImage.SetAlpha(0);
        amountImage.SetAlpha(0);

        if (item == null)
            return;

        if (item.currentAmount < 1)
        {
            item = null;

            return;
        }

        itemImage.sprite = item.icon;

        itemImage.SetAlpha(1);

        //Don't show amount icon and text if item is not stackable
        amountImage.SetAlpha(item.maxStackSize > 1 ? 1 : 0);
        amountText.text = item.maxStackSize > 1 ? item.currentAmount.ToString() : String.Empty;

        //Don't show durability bar if item is not breakable. Compute it if true
        durabilityBarImage.SetAlpha(item.hasDurability ? 1 : 0);
        durabilityBarFillImage.SetAlpha(item.hasDurability ? 1 : 0);

        float normalizedDurability = (float) item.currentDurability / item.maxDurability;
        durabilityBarFillImage.fillAmount = normalizedDurability;
        durabilityBarFillImage.color = Color.Lerp(durabilityMinColor, durabilityMaxColor, normalizedDurability);
    }

    public void SetItem(Item _item)
    {
        if (_item)
            item = _item.Clone();
        else
            item = null;

        OnItemChange?.Invoke(); 

        Refresh();
    }

    public void MouseClickEvent()
    {
        if (!owner.IsEnabled)
            return;

        Inventory.InventoryClickEventData data = new Inventory.InventoryClickEventData();

        data.clickType = GetClickType();
        data.clickedInventory = owner;
        data.action = GetAction(data.clickType);
        data.amount = GetAmount(data.action);
        data.clickedSlot = this;
        data.player = owner.Owner;
        data.cursor = data.player.GetCursor();
        data.hotbarBtn = -1;
        data.isCancelled = false;
        data.view = owner.View;

        owner.InvokeInventoryClick(data);
    }

    int GetAmount(Inventory.InventoryClickAction action)
    {
        switch (action)
        {
            case Inventory.InventoryClickAction.AllToCursor:
            case Inventory.InventoryClickAction.HalfToCursor:
            case Inventory.InventoryClickAction.OneToCursor:
            case Inventory.InventoryClickAction.Fast:
                return item == null ? 0 :  item.currentAmount;

            case Inventory.InventoryClickAction.PlaceAll:
            case Inventory.InventoryClickAction.PlaceHalf:
            case Inventory.InventoryClickAction.PlaceOne:
                return owner.View.GetCursor().GetItem() == null ? 
                    0 : owner.View.GetCursor().GetItem().currentAmount;

            default:
                return 0;
        }
    }

    Inventory.InventoryClickAction GetAction(CursorUtilities.ClickType clickType)
    {
        if (owner.View.GetCursor().IsEmpty)
        {
            if (item == null)
            {
                return Inventory.InventoryClickAction.None;
            }

            switch (clickType)
            {
                case CursorUtilities.ClickType.Right:
                    return Inventory.InventoryClickAction.HalfToCursor;
                case CursorUtilities.ClickType.Middle:
                    return Inventory.InventoryClickAction.OneToCursor;
                case CursorUtilities.ClickType.ShiftLeft:
                    return Inventory.InventoryClickAction.Fast;
                case CursorUtilities.ClickType.Left:
                    return Inventory.InventoryClickAction.AllToCursor;
                case CursorUtilities.ClickType.ShiftRight:
                    return Inventory.InventoryClickAction.None;
                case CursorUtilities.ClickType.None:
                    return Inventory.InventoryClickAction.None;
            }
        }
        else if (item != null && owner.View.GetCursor().GetItem().id != item.id)
        {
            switch (clickType)
            {
                case CursorUtilities.ClickType.Right:
                case CursorUtilities.ClickType.Middle:
                case CursorUtilities.ClickType.Left:
                    return Inventory.InventoryClickAction.AllToCursor;
                case CursorUtilities.ClickType.ShiftLeft:
                    return Inventory.InventoryClickAction.Fast;
                case CursorUtilities.ClickType.ShiftRight:
                case CursorUtilities.ClickType.None:
                    return Inventory.InventoryClickAction.None;
            }
        }

        switch (clickType)
        {
            case CursorUtilities.ClickType.Right:
                return Inventory.InventoryClickAction.PlaceHalf;
            case CursorUtilities.ClickType.Middle:
                return Inventory.InventoryClickAction.PlaceOne;
            case CursorUtilities.ClickType.ShiftLeft:
                return Inventory.InventoryClickAction.Fast;
            case CursorUtilities.ClickType.Left:
                return Inventory.InventoryClickAction.PlaceAll;
            case CursorUtilities.ClickType.ShiftRight:
                return Inventory.InventoryClickAction.None;
            case CursorUtilities.ClickType.None:
                return Inventory.InventoryClickAction.None;
        }

        return Inventory.InventoryClickAction.Out;
    }

    CursorUtilities.ClickType GetClickType()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            if (InputManager.Instance.Input.PlayerGround.Sprint.IsPressed())
                return CursorUtilities.ClickType.ShiftLeft;
            else
                return CursorUtilities.ClickType.Left;

        if (Mouse.current.rightButton.wasReleasedThisFrame)
            if (InputManager.Instance.Input.PlayerGround.Sprint.IsPressed())
                return CursorUtilities.ClickType.ShiftRight;
            else
                return CursorUtilities.ClickType.Right;

        if (Mouse.current.middleButton.wasReleasedThisFrame)
            return CursorUtilities.ClickType.Middle;

        return CursorUtilities.ClickType.None;
    }
}
