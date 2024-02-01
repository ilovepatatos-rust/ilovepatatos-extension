using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BattleRoyalGizmos
{
    [Serializable]
    public class Settings
    {
        public float Radius = 50;
        public ushort Thickness = 1;
    }

    public const string SPHERE_GIZMOS_PREFAB = Prefabs.SPHERE_VISUAL;
    public const string BR_SHADER_PREFAB = Prefabs.BATTLE_ROYAL_SHADER;

    private readonly List<SphereEntity> m_Gizmos = new();

    public void CreateAt(Vector3 pos, float radius, ushort thickness = 1, string prefab = SPHERE_GIZMOS_PREFAB)
    {
        CreateAt(prefab, pos, radius, thickness);
    }

    public void CreateAt(Vector3 pos, Settings settings, string prefab = SPHERE_GIZMOS_PREFAB)
    {
        CreateAt(pos, settings.Radius, settings.Thickness, prefab);
    }

    public void Kill()
    {
        m_Gizmos.KillAll();
    }

    public void SetSize(float radius)
    {
        foreach (SphereEntity gizmos in GetGizmos())
            gizmos.lerpRadius = radius;
    }

    public void SetPosition(Vector3 pos)
    {
        foreach (SphereEntity gizmos in GetGizmos())
            gizmos.ServerPosition = pos;
    }

    public void SetLerpSpeed(float speed)
    {
        foreach (SphereEntity gizmos in GetGizmos())
            gizmos.lerpSpeed = speed;
    }

    public bool Contains(Vector3 pos)
    {
        return m_Gizmos.Any(gizmos => Vector3.Distance(gizmos.ServerPosition, pos) <= gizmos.currentRadius);
    }

    public bool Contains(BaseEntity entity)
    {
        return entity != null && Contains(entity.ServerPosition);
    }

    private void CreateAt(string prefab, Vector3 pos, float radius, ushort thickness = 1)
    {
        for (int i = 0; i < thickness; i++)
        {
            var gizmos = GameManager.server.CreateEntity(prefab, pos) as SphereEntity;
            if (gizmos == null) break;

            gizmos.lerpRadius = radius;
            gizmos.lerpSpeed = float.MaxValue;

            gizmos.enableSaving = false;
            gizmos.Spawn();

            m_Gizmos.Add(gizmos);
        }
    }

    private IEnumerable<SphereEntity> GetGizmos()
    {
        return m_Gizmos.Where(x => x != null);
    }
}