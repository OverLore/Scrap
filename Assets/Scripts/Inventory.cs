using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Viewable
{
    [SerializeField] protected Transform grid;

    public List<Slot> slots = new List<Slot>();

    public event EventHandler<InventoryClickEventData> InventoryClickEvent;

    public FPSController Owner;

    public enum InventoryClickAction
    {
        DropAll,
        DropHalf,
        DropOne,
        HotbarSwap,
        HotbarSwapInventory,
        Out,
        AllToCursor,
        HalfToCursor,
        OneToCursor,
        SomeToCursor,
        PlaceAll,
        PlaceHalf,
        PlaceOne,
        PlaceSome,
        Fast,
        None
    }

    public class InventoryClickEventData : EventArgs
    {
        public View view;
        public Slot clickedSlot;
        public CursorSlot cursor;
        public FPSController player;
        public Inventory clickedInventory;
        public CursorUtilities.ClickType clickType;
        public InventoryClickAction action;
        public int hotbarBtn;
        public int amount;
        public bool isCancelled;

        public override string ToString()
        {
            return $"View : {view} " +
                   $"ClickedSlot : {clickedSlot} " +
                   $"Cursor : {cursor} " +
                   $"Player : {player} " +
                   $"Inventory : {clickedInventory} " +
                   $"ClickType : {clickType} " +
                   $"Action : {action} " +
                   $"Amount : {amount} " +
                   $"IsCancelled : {isCancelled} " +
                   $"HotbarButton : {hotbarBtn}";
        }
    }

    public void InvokeInventoryClick(InventoryClickEventData data)
    {
        InventoryClickEvent?.Invoke(this, data);
    }

    public override void Awake()
    {
        base.Awake();

        InventoryClickEvent += OnInventoryClickEvent;

        LoadSlots();
    }

    private void LoadSlots()
    {
        for (int i = 0; i < grid.childCount; i++)
        {
            slots.Add(grid.GetChild(i).GetComponent<Slot>());
            slots[i].SetOwner(this);
        }
    }

    public int AddItem(Item item)
    {
        if (!item || slots.Count <= 0)
        {
            return -1;
        }

        if (item.currentAmount <= 0)
        {
            return 0;
        }

        int amount = item.currentAmount;

        while (amount > 0)
        {
            Slot slotFound = null;

            slotFound = slots.FirstOrDefault(
                slot => slot.GetItem() != null
                        && slot.GetItem().id == item.id
                        && slot.GetItem().currentAmount < slot.GetItem().maxStackSize);

            if (slotFound != null)
            {
                int ToAdd = Mathf.Min(amount, slotFound.GetItem().maxStackSize - slotFound.GetItem().currentAmount);

                slotFound.GetItem().currentAmount += ToAdd;

                amount -= ToAdd;

                slotFound.Refresh();
            }
            else
            {
                slotFound = null;

                slotFound = slots.FirstOrDefault(slot => slot.GetItem() == null);

                if (!slotFound)
                    return amount;

                int ToAdd = Mathf.Min(amount, item.maxStackSize);

                item.currentAmount = ToAdd;

                slotFound.SetItem(item);

                amount -= ToAdd;

                slotFound.Refresh();
            }
        }

        return 0;
    }

    private void OnDestroy()
    {
        InventoryClickEvent -= OnInventoryClickEvent;
    }

    void SwapSlotAndCursor(Slot s, CursorSlot cs)
    {
        Item tempItem = s.GetItem();
        s.SetItem(cs.GetItem());
        cs.SetItem(tempItem);
    }

    void ActionToCursor(InventoryClickEventData data)
    {
        if (data.clickedSlot.IsEmpty)
            return;

        if (data.cursor.IsEmpty)
        {
            Item itemCopy = data.clickedSlot.GetItem().Clone();

            switch (data.action)
            {
                case InventoryClickAction.AllToCursor:
                    data.cursor.SetItem(data.clickedSlot.GetItem());
                    data.clickedSlot.SetItem(null);
                    break;
                case InventoryClickAction.HalfToCursor:
                    int amount = data.clickedSlot.GetItem().currentAmount / 2;

                    itemCopy.currentAmount = amount;
                    data.clickedSlot.GetItem().currentAmount -= amount;
                    data.cursor.SetItem(itemCopy);
                    break;
                case InventoryClickAction.OneToCursor:
                    itemCopy.currentAmount = 1;
                    data.clickedSlot.GetItem().currentAmount--;
                    data.cursor.SetItem(itemCopy);
                    break;
            }
        }
        else if (data.cursor.GetItem().id == data.clickedSlot.GetItem().id)
        {
            int amount = 0;

            switch (data.action)
            {
                case InventoryClickAction.AllToCursor:
                    amount = data.clickedSlot.GetItem().currentAmount;
                    break;
                case InventoryClickAction.HalfToCursor:
                    amount = data.clickedSlot.GetItem().currentAmount / 2;
                    break;
                case InventoryClickAction.OneToCursor:
                    amount = 1;
                    break;
            }

            int ToAdd = Mathf.Min(amount, data.cursor.GetItem().maxStackSize - data.cursor.GetItem().currentAmount);

            data.cursor.GetItem().currentAmount += ToAdd;
            data.clickedSlot.GetItem().currentAmount -= ToAdd;
        }
        else
        {
            SwapSlotAndCursor(data.clickedSlot, data.cursor);
        }

        data.clickedSlot.Refresh();
        data.cursor.Refresh();
    }

    void PlaceAction(InventoryClickEventData data)
    {
        if (data.cursor.IsEmpty)
            return;

        if (data.clickedSlot.IsEmpty)
        {
            Item itemCopy = data.cursor.GetItem().Clone();

            switch (data.action)
            {
                case InventoryClickAction.PlaceAll:
                    data.clickedSlot.SetItem(data.cursor.GetItem());
                    data.cursor.SetItem(null);
                    break;
                case InventoryClickAction.PlaceHalf:
                    int amount = data.cursor.GetItem().currentAmount / 2;

                    itemCopy.currentAmount = amount;
                    data.cursor.GetItem().currentAmount -= amount;
                    data.clickedSlot.SetItem(itemCopy);
                    break;
                case InventoryClickAction.PlaceOne:
                    itemCopy.currentAmount = 1;
                    data.cursor.GetItem().currentAmount--;
                    data.clickedSlot.SetItem(itemCopy);
                    break;
            }
        }
        else if (data.cursor.GetItem().id == data.clickedSlot.GetItem().id &&
                 !(data.cursor.IsFull || data.clickedSlot.IsFull))
        {
            int amount = 0;

            switch (data.action)
            {
                case InventoryClickAction.PlaceAll:
                    amount = data.cursor.GetItem().currentAmount;
                    break;
                case InventoryClickAction.PlaceHalf:
                    amount = data.cursor.GetItem().currentAmount / 2;
                    break;
                case InventoryClickAction.PlaceOne:
                    amount = 1;
                    break;
            }

            int ToAdd = Mathf.Min(amount,
                data.clickedSlot.GetItem().maxStackSize - data.clickedSlot.GetItem().currentAmount);

            data.clickedSlot.GetItem().currentAmount += ToAdd;
            data.cursor.GetItem().currentAmount -= ToAdd;
        }
        else
        {
            SwapSlotAndCursor(data.clickedSlot, data.cursor);
        }

        data.clickedSlot.Refresh();
        data.cursor.Refresh();
    }

    public virtual void DoFastAction(InventoryClickEventData data)
    {
        Inventory destination = View.HasMultipleInventories() ? Owner.GetHotbar() : View.GetFirstOtherInventory();

        data.clickedSlot.GetItem().currentAmount = destination.AddItem(data.clickedSlot.GetItem().Clone());

        data.clickedSlot.Refresh();
    }

    void OnInventoryClickEvent(object sender, InventoryClickEventData data)
    {
        Debug.Log(data);

        switch (data.action)
        {
            case InventoryClickAction.AllToCursor:
            case InventoryClickAction.HalfToCursor:
            case InventoryClickAction.OneToCursor:
                ActionToCursor(data);
                break;

            case InventoryClickAction.PlaceAll:
            case InventoryClickAction.PlaceHalf:
            case InventoryClickAction.PlaceOne:
                PlaceAction(data);
                break;

            case InventoryClickAction.Fast:
                DoFastAction(data);
                break;

            case InventoryClickAction.DropAll:
            case InventoryClickAction.DropHalf:
            case InventoryClickAction.DropOne:

            case InventoryClickAction.HotbarSwap:
            case InventoryClickAction.HotbarSwapInventory:

            case InventoryClickAction.Out:
            case InventoryClickAction.None:

            default:
                break;
        }
    }
}