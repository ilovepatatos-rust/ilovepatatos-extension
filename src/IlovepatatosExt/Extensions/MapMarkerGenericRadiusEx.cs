using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class MapMarkerGenericRadiusEx
{
    public static void SendUpdateToPlayer(this MapMarkerGenericRadius marker, BasePlayer player)
    {
        float a = marker.color1.a;
        float alpha = marker.alpha;
        float radius = marker.radius;

        var inner = marker.color1.ToVector3();
        var outer = marker.color2.ToVector3();

        marker.ClientRPCPlayer(null, player, "MarkerUpdate", inner, a, outer, alpha, radius);
    }
}