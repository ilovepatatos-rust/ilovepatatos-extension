using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ItemContainerEx
{
    public static int InstantClear(this ItemContainer container)
    {
        List<Item> items = container.itemList.ToPooledList();
        int count = items.Count;

        foreach (Item item in items)
            item.DoRemove();

        PoolUtility.Free(ref items);
        return count;
    }

    public static int GetAmountAvailableSlots(this ItemContainer container)
    {
        return container.capacity - container.itemList.Count;
    }

    public static int GetNextAvailableSlot(this ItemContainer container, Item item)
    {
        for (int i = 0; i < container.capacity; i++)
        {
            if (!container.SlotTaken(item, i))
                return i;
        }

        return -1;
    }

    public static int GiveAmount(this ItemContainer container, ItemDefinition def, int amountToGive, ulong skin)
    {
        foreach (Item item in container.itemList)
        {
            if (def.itemid != item.info.itemid) continue; // Skip wrong items
            if (skin != item.skin) continue; // Skip wrong skins

            int current = item.amount;
            int max = item.info.stackable;

            int space = max - current;
            if (space <= 0) continue; // Skip full stacks

            int toGive = Mathf.Min(amountToGive, space);

            item.amount += toGive;
            amountToGive -= toGive;

            if (amountToGive <= 0) return 0;
        }

        return amountToGive;
    }

    public static bool Contains(this ItemContainer container, int itemID)
    {
        return container.itemList != null && container.itemList.Any(x => x.info.itemid == itemID);
    }

    public static bool Contains(this ItemContainer container, string shortname)
    {
        ItemDefinition def = ItemManager.FindItemDefinition(shortname);
        return Contains(container, def.itemid);
    }

    public static bool ContainsIngredients(this ItemContainer container, int amount, IEnumerable<ItemAmount> ingredients)
    {
        foreach (ItemAmount ingredient in ingredients)
        {
            int count = container.GetAmount(ingredient.itemid, true);
            if (count < ingredient.amount * amount) return false;
        }

        return true;
    }

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