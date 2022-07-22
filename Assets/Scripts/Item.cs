using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Item/Default", fileName = "DefaultItem")]
public class Item : ScriptableObject
{
    [Header("Base")]
    public string id = string.Empty;
    public string commonName = string.Empty;
    public string pluralName = string.Empty;
    [TextArea] public string description = string.Empty;
    public Sprite icon = null;
    public string pickMessage = string.Empty;

    [Space(10), Header("Stack")]
    [Min(1)] public int maxStackSize = 1;
    public int currentAmount = 0;

    [Space(10), Header("Drop")]
    public bool isDroppable = false;
    public GameObject dropObject = null;

    [Space(10), Header("Hold")]
    public bool isHoldable = false;
    public GameObject inHandObject = null;

    [Space(10), Header("Food")]
    public bool isUsedAsFood = false;
    public int feedValue = 0;

    [Space(10), Header("Equipment")]
    public bool isUsedAsEquipment = false;
    public float asEquipmentProtection = 0;

    [Space(10), Header("Durability")]
    public bool hasDurability = false;
    public int maxDurability = 0;
    public int currentDurability = 0;

    [Space(10), Header("Craft")]
    public bool isCraftable = false;
    [TextArea] public string craftRecipe = string.Empty;

    [Space(10), Header("Furnace")]
    public bool isBakeable = false;
    [TextArea] public string bakeResult = string.Empty;

    /// <summary>
    /// <para><strong>Available placeholders :</strong></para>
    /// <list type="bullet">
    /// <item><em> %item_name% </em></item>
    /// <item><em> %item_max_stack% </em></item>
    /// <item><em> %item_amount%, </em></item>
    /// <item><em> %item_durability% </em></item>
    /// <item><em> %item_max_durability% </em></item>
    /// <item><em> %feed_value% </em></item>
    /// <item><em> %armor_value% </em></item>
    /// </list>
    /// </summary>
    /// <returns>The description of the item replacing placeholders by values</returns>
    public string GetFormattedDescription()
    {
        return description.Replace("%item_name%", currentAmount > 1 ? pluralName : commonName)
            .Replace("%item_max_stack%", maxStackSize.ToString())
            .Replace("%item_amount%", currentAmount.ToString())
            .Replace("%item_durability%", currentDurability.ToString())
            .Replace("%item_max_durability%", maxDurability.ToString())
            .Replace("%feed_value%", feedValue.ToString())
            .Replace("%armor_value%", Mathf.Round(asEquipmentProtection * 10).ToString());
    }

    /// <summary>
    /// <para><strong>Available placeholders :</strong></para>
    /// <list type="bullet">
    /// <item><em> %item_name% </em></item>
    /// <item><em> %item_amount%, </em></item>
    /// <item><em> %item_durability% </em></item>
    /// <item><em> %item_max_durability% </em></item>
    /// </list>
    /// </summary>
    /// <returns>The pick message of the item replacing placeholders by values</returns>
    public string GetFormattedPickMessage()
    {
        return pickMessage.Replace("%item_name%", currentAmount > 1 ? pluralName : commonName)
            .Replace("%item_amount%", currentAmount.ToString())
            .Replace("%item_durability%", currentDurability.ToString())
            .Replace("%item_max_durability%", maxDurability.ToString());
    }
    
    public Item Clone()
    {
        return (Item)MemberwiseClone();
    }
}
