using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StorageContainerEx
{
    public static bool IsFull(this StorageContainer container, bool checkForPartialStacks = false)
    {
        return container.inventory == null || container.inventory.IsFull(checkForPartialStacks);
    }
}