using JetBrains.Annotations;
using Network;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class GenericMarker
{
    public MapMarkerGenericRadius Marker;

    public void CreateAt(Vector3 pos, GenericMarkerSettings settings)
    {
        CreateAt(pos, settings.Radius, settings.Alpha, settings.InnerColor(), settings.OuterColor());
    }

    public void CreateAt(Vector3 pos, float radius, float alpha, Color inner, Color outer)
    {
        Kill();

        Marker = GameManager.server.CreateEntity(Prefabs.GENERIC_MARKER, pos) as MapMarkerGenericRadius;
        if (Marker == null) return;

        SetRadius(radius);
        SetAlpha(alpha);
        SetColors(inner, outer);

        Marker.enableSaving = false;
        Marker.Spawn();
        SendNetworkUpdate();
    }

    public void Kill()
    {
        if (Marker != null)
            Marker.Kill();
    }

    public void SetColors(Color inner, Color outer)
    {
        if (Marker == null) return;

        Marker.color1 = inner;
        Marker.color2 = outer;
    }

    public void SetAlpha(float alpha)
    {
        if (Marker != null)
            Marker.alpha = alpha;
    }

    public void SetRadius(float radius)
    {
        if (Marker != null)
            Marker.radius = radius / 325f;
    }

    public void SendNetworkUpdate()
    {
        if (Marker != null)
            Marker.SendUpdate();
    }

    public void SendAsSnapshotToNewPlayer(BasePlayer player)
    {
        if (player == null) return;

        Connection connection = player.Connection;
        if (connection == null) return;

        Marker.SendAsSnapshot(player.Connection, true);
        Marker.SendUpdate();
        Marker.SendUpdateToPlayer(player); // Required to send the colors to the client
    }
}