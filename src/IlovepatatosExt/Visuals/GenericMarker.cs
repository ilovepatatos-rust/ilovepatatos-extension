﻿using Facepunch;
using JetBrains.Annotations;
using Network;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class GenericMarker : Pool.IPooled
{
    [Serializable]
    public class Settings
    {
        public float Radius = 50;
        public float Alpha = 1;
        public string HexInnerColor = "#0f961d", HexOuterColor = "#000000";

        public Color InnerColor()
        {
            return ColorUtility.ParseHexString(HexInnerColor);
        }

        public Color OuterColor()
        {
            return ColorUtility.ParseHexString(HexOuterColor);
        }
    }

    public static readonly HashSet<GenericMarker> All = new();
    public MapMarkerGenericRadius Marker;

    public void CreateAt(Vector3 pos, Settings settings)
    {
        CreateAt(pos, settings.Radius, settings.Alpha, settings.InnerColor(), settings.OuterColor());
    }

    public void CreateAt(Vector3 pos, float radius, float alpha, Color inner, Color outer)
    {
        Kill();
        All.Add(this);

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
        All.Remove(this);

        Marker.KillOnce();
        Marker = null;
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
        if (player == null)
            return;

        Connection connection = player.Connection;
        if (connection == null) return;

        Marker.SendAsSnapshot(player.Connection, true);
        Marker.SendUpdateToPlayer(player); // Required to send the colors to the client
    }

    void Pool.IPooled.EnterPool()
    {
        Kill();
    }

    void Pool.IPooled.LeavePool() { }
}