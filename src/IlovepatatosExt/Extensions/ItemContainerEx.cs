using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ItemContainerEx
{
    public static int InstantClear(this ItemContainer container)
    {
        int count = 0;

        for (int i = container.itemList.Count - 1; i >= 0; i--)
        {
            Item item = container.itemList[i];
            if (item == null)
                continue;

            if (!item.IsValid())
                continue;

            count++;
            item.DoRemove();
        }

        return count;
    }

    [MustUseReturnValue]
    public static int GetAmountAvailableSlots(this ItemContainer container)
    {
        return container.capacity - container.itemList.Count;
    }

    [MustUseReturnValue]
    public static int GetNextAvailableSlot(this ItemContainer container, Item item)
    {
        for (int i = 0; i < container.capacity; i++)
        {
            if (!container.SlotTaken(item, i))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Insert as much as possible of an item as possible into a container.
    /// </summary>
    /// <returns>The amount remaining to give that didn't fit in the container.</returns>
    public static int GiveAmount(this ItemContainer container, ItemDefinition def, int amountToGive, ulong skin)
    {
        foreach (Item item in container.itemList)
        {
            if (def.itemid != item.info.itemid) // Skip wrong items
                continue;

            if (skin != item.skin) // Skip wrong skins
                continue;

            int current = item.amount;
            int max = item.info.stackable;

            int space = max - current;
            if (space <= 0) // Skip full stacks
                continue;

            int toGive = Mathf.Min(amountToGive, space);

            item.amount += toGive;
            amountToGive -= toGive;

            if (amountToGive <= 0)
                return 0;
        }

        return amountToGive;
    }

    [MustUseReturnValue]
    public static bool Contains(this ItemContainer container, int itemID)
    {
        return container.itemList != null && container.itemList.Any(x => x.info.itemid == itemID);
    }

    [MustUseReturnValue]
    public static bool Contains(this ItemContainer container, string shortname)
    {
        ItemDefinition def = ItemManager.FindItemDefinition(shortname);
        return def != null && Contains(container, def.itemid);
    }

    [MustUseReturnValue]
    public static bool ContainsIngredients(this ItemContainer container, int amount, IEnumerable<ItemAmount> ingredients)
    {
        foreach (ItemAmount ingredient in ingredients)
        {
            int count = container.GetAmount(ingredient.itemid, true);
            if (count < ingredient.amount * amount) return false;
        }

        return true;
    }

    [MustUseReturnValue]
    public static int GetMaxCraftableAmount(this ItemContainer container, int amount, IEnumerable<ItemAmount> ingredients)
    {
        int maxAmount = amount;

        foreach (ItemAmount ingredient in ingredients)
        {
            int amountOf = container.GetAmount(ingredient.itemid, true);
            int max = amountOf / (int)ingredient.amount;

            maxAmount = Math.Min(max, maxAmount);
        }

        return Math.Max(0, maxAmount);
    }
}