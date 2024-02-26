using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt
{
    [Serializable]
    public class Entity
    {
        [JsonProperty("Prefab")]
        public string Prefab;

        [SerializeField, JsonProperty("Position")]
        private Vector3 m_InternalPosition;

        [SerializeField, JsonProperty("Rotation")]
        private Vector3 m_InternalRotation;

        [JsonIgnore]
        public Vector3 Pos
        {
            get => m_InternalPosition;
            set => m_InternalPosition = value;
        }

        [JsonIgnore]
        public Quaternion Rot
        {
            get => Quaternion.Euler(m_InternalRotation);
            set => m_InternalRotation = value.eulerAngles;
        }

        public BaseEntity CreateAtPivot(Vector3 pivotPos, Quaternion pivotRot, bool clipToTerrain = false)
        {
            Vector3 offsetPos = pivotRot * Pos + pivotPos;
            Quaternion offsetRot = pivotRot * Rot;

            if (clipToTerrain)
                offsetPos.y = MapUtility.GetTerrainHeightAt(offsetPos);

            return GameManager.server.CreateEntity(Prefab, offsetPos, offsetRot);
        }
    }
}