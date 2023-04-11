public class GatherableTree : Gatherable
{
    public override Item Gather(Item tool)
    {
        Item item = base.Gather(tool);

        InventoryController ic = InventoryController.Instance;

        if (item != null)
        {
            NotificationController.Instance.AddNotification("WoodCutting", $"+{item.currentStackAmount} {item.displayName}",
                $"(x{ic.inventory.GetTotalOfThisItem(item) + ic.hotbar.GetTotalOfThisItem(item)})", 5f, 
                false, () => { });
        }
        
        return item;
    }
}
