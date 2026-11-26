using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class VendingMachineEx
{
    extension(VendingMachine vending)
    {
        public void SetTitle(string title)
        {
            vending.shopName = title;
            vending.FullUpdate();
        }
    }
}