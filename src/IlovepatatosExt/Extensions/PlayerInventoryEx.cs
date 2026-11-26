using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerInventoryEx
{
    extension(PlayerInventory inventory)
    {
        [MustUseReturnValue]
        public bool IsEmpty()
        {
            if (!inventory.containerWear.IsEmpty())
                return false;

            if (!inventory.containerMain.IsEmpty())
                return false;

            if (!inventory.containerBelt.IsEmpty())
                return false;

            return true;
        }

        [MustUseReturnValue]
        public bool IsFull()
        {
            return inventory.containerMain.IsFull() && inventory.containerBelt.IsFull();
        }

        [MustUseReturnValue]
        public int GetAmount(int itemId, bool onlyUsable = false)
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

        [MustUseReturnValue]
        public int GetAmount(string shortname, bool onlyUsable = false)
        {
            ItemDefinition def = ItemManager.FindItemDefinition(shortname);
            return def == null ? 0 : inventory.GetAmount(def.itemid, onlyUsable);
        }

        [MustUseReturnValue]
        public bool HasAmount(int itemId, int amount, bool onlyUsable = false)
        {
            int count = inventory.GetAmount(itemId, onlyUsable);
            return count >= amount;
        }

        public int Take(int itemId, int amount)
        {
            var items = PoolUtility.Get<List<Item>>();
            int count = inventory.Take(items, itemId, amount);

            foreach (Item item in items)
                item.Remove();

            if (count > 0)
            {
                BasePlayer owner = inventory.GetBaseEntity() as BasePlayer;

                if (owner != null && owner.IsConnected)
                    owner.Command("note.inv", itemId, -count);
            }

            PoolUtility.Free(ref items);
            return count;
        }

        public int Take(string shortname, int amount)
        {
            ItemDefinition def = ItemManager.FindItemDefinition(shortname);
            return def != null ? inventory.Take(def.itemid, amount) : 0;
        }

        [MustUseReturnValue]
        public bool HasInventorySpaceFor(string shortname, int amount)
        {
            ItemDefinition def = ItemManager.FindItemDefinition(shortname);
            if (def == null) return false;

            ItemContainer main = inventory.containerMain;
            ItemContainer belt = inventory.containerBelt;

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
}