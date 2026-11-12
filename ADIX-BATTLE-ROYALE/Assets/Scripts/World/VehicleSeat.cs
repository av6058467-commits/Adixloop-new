using UnityEngine;

namespace AdixBattleRoyale.World
{
    /// <summary>Basic enter/exit seat anchor. Vehicle physics is handled by VehicleDrive.</summary>
    public sealed class VehicleSeat : MonoBehaviour
    {
        [SerializeField] private Transform seatAnchor;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private VehicleDrive vehicle;
        public Transform SeatAnchor => seatAnchor != null ? seatAnchor : transform;
        public Transform ExitPoint => exitPoint;
        public VehicleDrive Vehicle => vehicle;

        private void Reset()
        {
            if (vehicle == null) vehicle = GetComponentInParent<VehicleDrive>();
        }
    }
}