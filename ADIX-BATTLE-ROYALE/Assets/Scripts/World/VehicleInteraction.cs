using UnityEngine;
using AdixBattleRoyale.Player;

namespace AdixBattleRoyale.World
{
    /// <summary>Connect an interact button to EnterOrExitVehicle.</summary>
    public sealed class VehicleInteraction : MonoBehaviour
    {
        [SerializeField] private MobilePlayerController playerController;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform playerRoot;
        [SerializeField, Min(1f)] private float searchRadius = 3.5f;
        private VehicleSeat activeSeat;
        private bool inVehicle;

        public bool InVehicle => inVehicle;

        public void EnterOrExitVehicle()
        {
            if (playerRoot == null || playerController == null) return;
            if (inVehicle) ExitVehicle();
            else EnterNearestVehicle();
        }

        private void EnterNearestVehicle()
        {
            VehicleSeat[] seats = FindObjectsByType<VehicleSeat>(FindObjectsSortMode.None);
            float bestDistance = searchRadius;
            VehicleSeat best = null;
            foreach (VehicleSeat seat in seats)
            {
                if (seat == null || seat.Vehicle == null || seat.Vehicle.HasDriver) continue;
                float distance = Vector3.Distance(playerRoot.position, seat.SeatAnchor.position);
                if (distance < bestDistance) { bestDistance = distance; best = seat; }
            }
            if (best == null) return;

            activeSeat = best;
            inVehicle = true;
            if (characterController != null) characterController.enabled = false;
            playerRoot.SetParent(activeSeat.SeatAnchor, false);
            playerRoot.localPosition = Vector3.zero;
            playerRoot.localRotation = Quaternion.identity;
            activeSeat.Vehicle.SetDriver(true);
            playerController.enabled = false;
        }

        private void ExitVehicle()
        {
            if (activeSeat == null) { inVehicle = false; return; }
            Transform exit = activeSeat.ExitPoint;
            playerRoot.SetParent(null, true);
            if (exit != null) playerRoot.position = exit.position;
            else playerRoot.position = activeSeat.transform.position + activeSeat.transform.right * 2f;
            activeSeat.Vehicle.SetDriver(false);
            activeSeat = null;
            inVehicle = false;
            playerController.enabled = true;
            if (characterController != null) characterController.enabled = true;
        }
    }
}