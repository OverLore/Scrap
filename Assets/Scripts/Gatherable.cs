using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gatherable : MonoBehaviour, IHitDecallabe
{
    [System.Serializable]
    class ItemDecal
    {
        public IHitDecallabe.HitType type;
        public GameObject impactDecal;
    }

    [System.Serializable]
    class ItemEfficiency
    {
        public Item item;
        public int minItemAmount;
        public int maxItemAmount;
        public int damage;
    }

    [Header("Decal")] 
    [SerializeField] private List<ItemDecal> decals;
    [SerializeField] private float projectorOffset;
    [SerializeField, Range(0, 180)] private float randomAngleLimits;

    [Space(10), Header("Gathering")]
    [SerializeField] private Item gatheredItem;
    [SerializeField] private int life;
    [SerializeField] private List<ItemEfficiency> usableTools;

    GameObject GetDecal(IHitDecallabe.HitType type)
    {
        foreach (ItemDecal decal in decals)
        {
            if (decal.type == type)
                return decal.impactDecal;
        }

        return null;
    }

    int GetEfficiency(Item item)
    {
        foreach (ItemEfficiency eff in usableTools)
        {
            if (eff.item.id == item.id)
                return Random.Range(eff.minItemAmount, eff.maxItemAmount);
        }

        return 0;
    }

    int GetDamage(Item item)
    {
        foreach (ItemEfficiency eff in usableTools)
        {
            if (eff.item.id == item.id)
                return eff.damage;
        }

        return 0;
    }

    public void SetDecal(RaycastHit hit, IHitDecallabe.HitType type)
    {
        GameObject go = Instantiate(GetDecal(type));

        go.transform.position = hit.point + hit.normal * projectorOffset;
        go.transform.LookAt(hit.point);
        go.transform.Rotate(go.transform.forward, Random.Range(-randomAngleLimits, randomAngleLimits));

        Destroy(go, 10f);
    }

    void Kill()
    {
        //TODO : Fall animation ? Give an item boost ?
        Destroy(gameObject);
    }

    public void Gather(FPSController player)
    {
        Slot usedSlot = player.GetHotbar().CurrentSlot;
        Item usedItem = usedSlot.GetItem();

        int amountToGive = Mathf.Min(GetEfficiency(usedItem), life);

        if (amountToGive == 0)
        {
            //TODO: Show a message to inform player that hé is using a wrong tool
        }

        if (usedItem.hasDurability)
        {
            usedItem.currentDurability--;

            usedSlot.Refresh();
        }

        life -= GetDamage(usedItem);

        if (life < 1)
            Kill();

        Item toAdd = Instantiate(gatheredItem);
        gatheredItem.currentAmount = amountToGive;

        player.GetNotificationManager().AddNotification(NotificationManager.NotificationType.Normal,
            NotificationManager.NotificationStyle.InventoryAdd, gatheredItem.commonName, amountToGive.ToString());

        player.GetInventory().AddItem(toAdd);
    }
}
