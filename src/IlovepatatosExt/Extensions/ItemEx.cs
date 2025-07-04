﻿using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ItemEx
{
    public static void SetSkin(this Item item, ulong skinID)
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