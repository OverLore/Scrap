using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour, IInteractable, ISaveable<Pickable.PickableData>
{
    [System.Serializable]
    public struct PickableData
    {
        public Item.ItemData refItem;
        public Vector3 position;
    }

    [Header("Infos")] 
    [SerializeField] public Item item;
    [SerializeField] public int itemAmount;

    private void Awake()
    {
        item.currentStackAmount = itemAmount;
    }

    public void DoInteraction()
    {
        Item it = InventoryController.Instance.inventory.AddItem(item);

        if (it == null)
            Destroy(gameObject);
        else
            item = it;
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();

        if (item == null)
        {
            ActionUI.Instance.SetText("Unknown item");
            return;
        }

        ActionUI.Instance.SetText($"Press E to pick x{item.currentStackAmount} {item.displayName}");
    }

    private void OnDestroy()
    {
        PickableManager.Instance.pickableList.Remove(this);
    }

    public PickableData CreateSaveData()
    {
        Pickable.PickableData pickableData = new Pickable.PickableData();

        pickableData.refItem = item.CreateSaveData();
        pickableData.position = transform.position;

        return pickableData;
    }

    public void ReadSaveData(PickableData _data)
    {

    }

    public string GetFileName()
    {
        return typeof(Pickable).ToString();
    }
}
