using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerInventoryEx
{
    public static bool IsFull(this PlayerInventory inventory)
    {
        return inventory.containerMain.IsFull() && inventory.containerBelt.IsFull();
    }

    public static int GetAmount(this PlayerInventory inventory, int itemId, bool onlyUsable = false)
    {
        int amount = 0;

        if (inventory.containerMain != null)
            amount += inventory.containerMain.GetAmount(itemId, onlyUsable);

        if (inventory.containerBelt != null)
            amount += inventory.containerBelt.GetAmount(itemId, onlyUsable);

        if (inventory.containerWear != null)
            amount += inventory.containerWear.GetAmount(itemId, onlyUsable);

        return amount;
    }

    public static int GetAmount(this PlayerInventory inventory, string shortname, bool onlyUsable = false)
    {
        ItemDefinition def = ItemManager.FindItemDefinition(shortname);
        return def == null ? 0 : inventory.GetAmount(def.itemid, onlyUsable);
    }

    public static bool HasAmount(this PlayerInventory inventory, int itemId, int amount, bool onlyUsable = false)
    {
        int count = inventory.GetAmount(itemId, onlyUsable);
        return count >= amount;
    }

    public static int Take(this PlayerInventory inventory, int itemId, int amount)
    {
        var items = PoolUtility.Get<List<Item>>();
        int count = inventory.Take(items, itemId, amount);

        foreach (Item item in items)
            item.Remove();

        if (count > 0)
        {
            BasePlayer owner = inventory._baseEntity;

            if (owner != null && owner.IsConnected)
                owner.Command("note.inv", itemId, -count);
        }

        PoolUtility.Free(ref items);
        return count;
    }

    public static int Take(this PlayerInventory inventory, string shortname, int amount)
    {
        ItemDefinition def = ItemManager.FindItemDefinition(shortname);
        return def != null ? inventory.Take(def.itemid, amount) : 0;
    }

    public static bool HasInventorySpaceFor(this PlayerInventory inventory, string shortname, int amount)
    {
        var def = ItemManager.FindItemDefinition(shortname);
        if (def == null) return false;

        var main = inventory.containerMain;
        var belt = inventory.containerBelt;

        // Complete all stacks in main container
        foreach (Item item in main.itemList)
        {
            if (item.info != null && string.Equals(item.info.shortname, shortname))
                amount -= Math.Max(0, item.info.stackable - item.amount);
        }

        if (amount <= 0)
            return true;

        // Complete all stacks in belt container
        foreach (Item item in belt.itemList)
        {
            if (item.info != null && string.Equals(item.info.shortname, shortname))
                amount -= Math.Max(0, item.info.stackable - item.amount);
        }

        if (amount <= 0)
            return true;

        // Search for inventory space for remaining stack(s)
        int amountStacks = Mathf.CeilToInt((float)amount / def.stackable);

        int amountSlotsTaken = main.itemList.Count + belt.itemList.Count;
        int amountSlotsTotal = main.capacity + belt.capacity;

        bool hasSpace = amountStacks <= amountSlotsTotal - amountSlotsTaken;
        return hasSpace;
    }
}