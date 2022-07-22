using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hotbar : Inventory
{
    [SerializeField] private Image selectionImage;

    public Slot CurrentSlot => selectedSlot > -1 ? slots[selectedSlot] : null;

    private int selectedSlot = -1;
    private int lastSelectedSlot = -1;

    float nextAllowedSwitchIn;
    float switchDelay = 0.02f;

    public override void DoFastAction(InventoryClickEventData data)
    {
        Inventory destination = View.HasMultipleInventories() ? Owner.GetInventory() : View.GetFirstOtherInventory();

        data.clickedSlot.GetItem().currentAmount = destination.AddItem(data.clickedSlot.GetItem().Clone());
    
        data.clickedSlot.Refresh();
    }

    void DoDelayCountdown()
    {
        if (nextAllowedSwitchIn > 0)
            nextAllowedSwitchIn -= Time.deltaTime;
    }

    void HandleScroll()
    {
        float yInput = Mouse.current.scroll.ReadValue().y;

        if (nextAllowedSwitchIn <= 0 && yInput != 0)
        {
            nextAllowedSwitchIn = switchDelay;
            selectedSlot += yInput < 0 ? 1 : -1;

            if (selectedSlot < 0)
                selectedSlot = slots.Count - 1;
            else if (selectedSlot > slots.Count - 1)
                selectedSlot = 0;

            Refresh();
        }
    }

    private void Unequip()
    {
        Transform handRoot = Owner.GetHandRoot();

        int childs = handRoot.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(handRoot.GetChild(i).gameObject);
        }
    }

    private void Equip()
    {
        Transform handRoot = Owner.GetHandRoot();

        Unequip();

        GameObject go = Instantiate(slots[selectedSlot].GetItem().inHandObject, handRoot);

        HoldableItem holdable = go.GetComponent<HoldableItem>();

        if (holdable == null)
            return;

        holdable.SetOwner(Owner);
    }

    void HandleFastSelection()
    {
        int fastSlot = 0;

        if (InputManager.Instance.Input.PlayerGround.Slot1.triggered)
            fastSlot = 1;
        if (InputManager.Instance.Input.PlayerGround.Slot2.triggered)
            fastSlot = 2;
        if (InputManager.Instance.Input.PlayerGround.Slot3.triggered)
            fastSlot = 3;
        if (InputManager.Instance.Input.PlayerGround.Slot4.triggered)
            fastSlot = 4;
        if (InputManager.Instance.Input.PlayerGround.Slot5.triggered)
            fastSlot = 5;
        if (InputManager.Instance.Input.PlayerGround.Slot6.triggered)
            fastSlot = 6;
        if (InputManager.Instance.Input.PlayerGround.Slot7.triggered)
            fastSlot = 7;

        if (fastSlot > 0)
        {
            selectedSlot = fastSlot - 1;

            Refresh();
        }
    }

    private void Update()
    {
        DoDelayCountdown();

        if (View.IsEnabled)
            return;

        HandleScroll();
        HandleFastSelection();
    }

    void PlaceSelectionImage()
    {
        if (selectedSlot == -1)
        {
            selectionImage.SetAlpha(0);

            return;
        }

        selectionImage.SetAlpha(1);

        selectionImage.rectTransform.SetPositionAndRotation(slots[selectedSlot].GetComponent<Image>().rectTransform.position, Quaternion.identity);
    }

    void Refresh()
    {
        PlaceSelectionImage();

        if (selectedSlot == -1)
            return;

        if (lastSelectedSlot > -1)
            slots[lastSelectedSlot].OnItemChange -= Refresh;

        slots[selectedSlot].OnItemChange += Refresh;

        if (lastSelectedSlot != selectedSlot)
            slots[selectedSlot].PlayTriggerPop();

        Unequip();

        if (slots[selectedSlot].GetItem() != null && slots[selectedSlot].GetItem().isHoldable)
            Equip();

        lastSelectedSlot = selectedSlot;
    }
}
