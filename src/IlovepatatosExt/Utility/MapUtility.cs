using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class MapUtility
{
    public const int TERRAIN_MASK = (1 << (int)Rust.Layer.Default) | (1 << (int)Rust.Layer.World) |
                                    (1 << (int)Rust.Layer.Terrain) | (1 << (int)Rust.Layer.Prevent_Movement);

    public const int TERRAIN_ENTITY_MASK = (1 << (int)Rust.Layer.Default) | (1 << (int)Rust.Layer.World) |
                                           (1 << (int)Rust.Layer.Construction) | (1 << (int)Rust.Layer.Terrain) |
                                           (1 << (int)Rust.Layer.Transparent) | (1 << (int)Rust.Layer.Vehicle_Large) |
                                           (1 << (int)Rust.Layer.Tree);

    public static string ToGrid(Vector3 pos)
    {
        var half = new Vector2(pos.x + World.Size / 2f, pos.z + World.Size / 2f);
        int maxGridSize = Mathf.FloorToInt(World.Size / 146.3f) - 1;

        int x = Mathf.FloorToInt(half.x / 146.3f);
        int y = Mathf.FloorToInt(half.y / 146.3f);

        int num1 = Mathf.Clamp(x, 0, maxGridSize);
        int num2 = Mathf.Clamp(maxGridSize - y, 0, maxGridSize);

        string extraA = num1 > 25 ? $"{(char)('A' + (num1 / 26 - 1))}" : string.Empty;
        return $"{extraA}{(char)('A' + num1 % 26)}{num2}";
    }

    public static float GetTerrainHeightAt(Vector3 pos, float yOffset = 10, float range = 1000f, int mask = TERRAIN_MASK)
    {
        float water = TerrainMeta.WaterMap.GetHeight(pos);

        Vector3 offset = new(0, yOffset, 0);
        if (!TransformUtil.GetGroundInfo(pos + offset, out RaycastHit hit, range, mask))
            return Mathf.Max(pos.y, water);

        return Mathf.Max(hit.point.y, water);
    }

    public static bool ContainsTopologyAt(Vector3 pos, TerrainTopology.Enum topology)
    {
        int layer = TerrainMeta.TopologyMap.GetTopology(pos);
        return layer.ContainsTopology(topology);
    }
}