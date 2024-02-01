using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BaseEntityEx
{
    public static IEnumerable<T> FilterByTopologyBiome<T>(this IEnumerable<T> values,
        TerrainTopology.Enum targetTopology, TerrainBiome.Enum targetBiome) where T : BaseEntity
    {
        foreach (T entity in values.Where(x => x != null))
        {
            Vector3 pos = entity.transform.position;

            int topology = TerrainMeta.TopologyMap.GetTopology(pos);
            if (!topology.ContainsTopology(targetTopology))
                continue;

            int biome = TerrainMeta.BiomeMap.GetBiomeMaxType(pos);
            if ((TerrainBiome.Enum)biome != targetBiome)
                continue;

            yield return entity;
        }
    }

    public static T GetClosestOf<T>(this BaseEntity entity, float radius) where T : BaseNetworkable
    {
        Vector3 pos = entity.transform.position;

        var list = PoolUtility.Get<List<T>>();
        Vis.Entities(pos, radius, list);

        float smallestDistance = float.MaxValue;
        T closest = null;

        foreach (T other in list)
        {
            if (other == null || other.IsDestroyed)
                continue;

            float distance = Vector3.Distance(pos, other.transform.position);
            if (distance > smallestDistance) continue;

            smallestDistance = distance;
            closest = other;
        }

        PoolUtility.Free(ref list);
        return closest;
    }
}