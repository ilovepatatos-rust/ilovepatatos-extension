using JetBrains.Annotations;

namespace Oxide.Ext.AtlasExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ItemEx
{
    public static void SetSkin(this Item item, ulong skinID)
    {
        if (item.skin == skinID)
            return;

        item.skin = skinID;
        item.MarkDirty();
    }
}