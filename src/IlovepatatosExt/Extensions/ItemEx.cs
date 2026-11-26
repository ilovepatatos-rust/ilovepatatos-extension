using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ItemEx
{
    extension(Item item)
    {
        public void SetSkin(ulong skinID)
        {
            if (item.skin == skinID)
                return;

            item.skin = skinID;

            BaseEntity held = item.GetHeldEntity();
            if (held != null)
                held.skinID = skinID;

            item.MarkDirty();
        }
    }
}