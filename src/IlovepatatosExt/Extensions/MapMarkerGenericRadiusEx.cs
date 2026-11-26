using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class MapMarkerGenericRadiusEx
{
    extension(MapMarkerGenericRadius marker)
    {
        public void SendUpdateToPlayer(BasePlayer player)
        {
            var inner = marker.color1.ToVector3();
            var outer = marker.color2.ToVector3();

            RpcTarget target = RpcTarget.Player("MarkerUpdate", player);
            marker.ClientRPC(target, inner, marker.color1.a, outer, marker.alpha, marker.radius);
        }
    }
}