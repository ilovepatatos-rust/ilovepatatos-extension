using JetBrains.Annotations;
using Rust;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class HitInfoEx
{
    extension(HitInfo info)
    {
        [MustUseReturnValue]
        public bool IsSuicide()
        {
            return info.damageTypes.GetMajorityDamageType() == DamageType.Suicide;
        }
    }
}