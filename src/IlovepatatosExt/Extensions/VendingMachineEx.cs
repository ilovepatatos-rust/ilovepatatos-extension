using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class VendingMachineEx
    {
        public static void SetTitle(this VendingMachine vending, string title)
        {
            vending.shopName = title;
            vending.FullUpdate();
        }
    }
}