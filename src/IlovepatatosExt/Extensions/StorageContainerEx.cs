using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StorageContainerEx
{
    extension(StorageContainer container)
    {
        [MustUseReturnValue]
        public bool IsFull(bool checkForPartialStacks = false)
        {
            return container.inventory == null || container.inventory.IsFull(checkForPartialStacks);
        }
    }
}