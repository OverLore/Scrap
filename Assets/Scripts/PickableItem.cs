using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    [Header("Debug")]
    [SerializeField] private Item itemToSet;
    [SerializeField] private int amountToSet;

    [Space(10), Header("Item")]
    [SerializeField] private Item item;

    private void Awake()
    {
        if (itemToSet != null)
        {
            item = Instantiate(itemToSet);
            item.currentAmount = amountToSet;
        }
    }

    void PickUpItem(Inventory inventory)
    {
        item.currentAmount = inventory.AddItem(item);

        if (item.currentAmount == 0)
            Destroy(gameObject);
    }

    public void SetQuantity(int amount)
    {
        item.currentAmount = amount;
    }

    public void PerformInteraction(FPSController player)
    {
        PickUpItem(player.GetInventory());
    }

    public void SetInteractionText(InteractionText text)
    {
        text.ShowText(item.GetFormattedPickMessage());
    }
}
