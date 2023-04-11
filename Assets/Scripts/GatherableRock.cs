public class GatherableRock : Gatherable
{
    public override Item Gather(Item tool)
    {
        var item = base.Gather(tool);

        var ic = InventoryController.Instance;

        if (item != null)
        {
            NotificationController.Instance.AddNotification("StoneGathering", $"+{item.currentStackAmount} {item.displayName}",
                $"(x{ic.inventory.GetTotalOfThisItem(item) + ic.hotbar.GetTotalOfThisItem(item)})", 5f,
                false, () => { });
        }

        return item;
    }
}
