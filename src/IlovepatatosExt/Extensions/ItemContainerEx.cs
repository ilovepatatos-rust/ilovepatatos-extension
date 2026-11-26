using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ItemContainerEx
{
    extension(ItemContainer container)
    {
        public int InstantClear()
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
        public int GetAmountAvailableSlots()
        {
            return container.capacity - container.itemList.Count;
        }

        [MustUseReturnValue]
        public int GetNextAvailableSlot(Item item)
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
        public int GiveAmount(ItemDefinition def, int amountToGive, ulong skin)
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
        public bool Contains(int itemID)
        {
            return container.itemList != null && container.itemList.Any(x => x.info.itemid == itemID);
        }

        [MustUseReturnValue]
        public bool Contains(string shortname)
        {
            ItemDefinition def = ItemManager.FindItemDefinition(shortname);
            return def != null && Contains(container, def.itemid);
        }

        [MustUseReturnValue]
        public bool ContainsIngredients(int amount, IEnumerable<ItemAmount> ingredients)
        {
            foreach (ItemAmount ingredient in ingredients)
            {
                int count = container.GetAmount(ingredient.itemid, true);
                if (count < ingredient.amount * amount) return false;
            }

            return true;
        }

        [MustUseReturnValue]
        public int GetMaxCraftableAmount(int amount, IEnumerable<ItemAmount> ingredients)
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
}