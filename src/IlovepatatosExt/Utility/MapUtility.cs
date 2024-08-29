using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Parallel = UnityEngine.Parallel;

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

    private const float MAX_DELAY = 30f;

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

    public static IEnumerator KillNetworkablesOverTime<T>(Vector3 pos, float radius, float delay, Action<int, long> onComplete = null)
        where T : BaseNetworkable
    {
        var all = PoolUtility.Get<List<T>>();
        Vis.Entities(pos, radius, all);

        yield return KillNetworkablesOverTime(all, delay, onComplete);

        PoolUtility.Free(ref all);
    }

    public static IEnumerator KillNetworkablesOverTimeWhitelist(Vector3 pos, float radius, float delay,
        ICollection<uint> whitelist, Action<int, long> onComplete = null)
    {
        var all = PoolUtility.Get<List<BaseNetworkable>>();
        Vis.Entities(pos, radius, all);

        IEnumerable<BaseNetworkable> toRemove = all.Where(networkable => whitelist.Contains(networkable.prefabID));
        yield return KillNetworkablesOverTime(toRemove, delay, onComplete);

        PoolUtility.Free(ref all);
    }

    public static IEnumerator KillNetworkablesOverTimeBlacklist(Vector3 pos, float radius, float delay,
        ICollection<uint> blacklist, Action<int, long> onComplete = null)
    {
        var all = PoolUtility.Get<List<BaseNetworkable>>();
        Vis.Entities(pos, radius, all);

        IEnumerable<BaseNetworkable> toRemove = all.Where(networkable => !blacklist.Contains(networkable.prefabID));
        yield return KillNetworkablesOverTime(toRemove, delay, onComplete);

        PoolUtility.Free(ref all);
    }

    public static IEnumerator KillNetworkablesOverTime(IEnumerable<BaseNetworkable> networkables, float delay, Action<int, long> onComplete)
    {
        int killed = 0;
        var stopwatch = Stopwatch.StartNew();
        var waitFor = new WaitForSeconds(delay);

        foreach (BaseNetworkable networkable in networkables)
        {
            if (!KillNetworkable(networkable))
                continue;

            killed++;

            if (delay > 0)
                yield return waitFor;
        }

        stopwatch.Stop();
        onComplete?.Invoke(killed, stopwatch.Elapsed.Seconds);
    }

    /// <summary>
    /// Create spawn points on a topology inside a rect at a given position and size.
    /// </summary>
    /// <returns>Returns a list of locations and the time it took in milliseconds.</returns>
    public static void CreateSpawnPointsOnTopology(Vector3 center, float lenght,
        TerrainTopology.Enum targetTopology, int maxAmount, Action<List<Vector3>, long> onComplete)
    {
        Rect rect = CreateRectAt(center, lenght);
        var watch = Stopwatch.StartNew();

        IEnumerator task = GetAllAvailableSpawnPointsWithin(rect, targetTopology, locations =>
        {
            List<Vector3> list = locations.ToPooledList();

            uint seed = (uint)new System.Random().Next();
            list.Shuffle(seed);

            var values = new List<Vector3>(maxAmount);

            foreach (Vector3 x in list.Take(maxAmount))
            {
                Vector3 pos = x;
                pos.y = GetTerrainHeightAt(pos, 1000);
                values.Add(pos);
            }

            PoolUtility.Free(ref list);

            watch.Stop();
            onComplete?.Invoke(values, watch.ElapsedMilliseconds);
        });

        CoroutineUtility.StartCoroutine(task);
    }

    private static bool KillNetworkable(BaseNetworkable networkable)
    {
        if (networkable == null || networkable.IsDestroyed)
            return false;

        switch (networkable)
        {
            case StorageContainer container:
            {
                container.inventory?.Clear();
                break;
            }
            case DroppedItemContainer container:
            {
                container.inventory?.Clear();
                break;
            }
            case LootableCorpse corpse:
            {
                foreach (ItemContainer container in corpse.containers)
                    container.Kill();
                break;
            }
        }

        networkable.Kill();
        return true;
    }

    private static Rect CreateRectAt(Vector3 center, float lenght)
    {
        var initial = new Vector2(center.x, center.z);
        Vector2 topLeftCorner = initial + new Vector2(-lenght, lenght);
        Vector2 bottomRightCorner = initial + new Vector2(lenght, -lenght);

        var pos = new Vector2(topLeftCorner.x, bottomRightCorner.y);
        var size = new Vector2(bottomRightCorner.x - topLeftCorner.x, topLeftCorner.y - bottomRightCorner.y);
        return new Rect(pos, size);
    }

    private static IEnumerator GetAllAvailableSpawnPointsWithin(Rect rect, TerrainTopology.Enum targetTopology,
        Action<ConcurrentBag<Vector3>> onComplete)
    {
        int xMin = (int)rect.xMin;
        int xMax = (int)rect.xMax;
        int yMin = (int)rect.yMin;
        int yMax = (int)rect.yMax;

        int size = (xMax - xMin) * (yMax - yMin);
        var list = new ConcurrentBag<Vector3>();

        int count = 0;
        TimeSince timeSince = 0;

        Parallel.For(0, Math.Abs(xMax - xMin), x =>
        {
            for (int z = yMin; z < yMax; z++)
            {
                Interlocked.Increment(ref count);

                var pos = new Vector3(x + xMin, 0, z);
                int topology = TerrainMeta.TopologyMap.GetTopology(pos);

                if (!topology.ContainsTopology(targetTopology))
                    continue;

                list.Add(pos);
            }
        });

        while (count < size && timeSince < MAX_DELAY)
            yield return null;

        onComplete.Invoke(list);
    }
}