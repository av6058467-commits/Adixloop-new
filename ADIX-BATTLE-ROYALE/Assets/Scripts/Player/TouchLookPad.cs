using UnityEngine;
using UnityEngine.EventSystems;

namespace AdixBattleRoyale.Player
{
    /// <summary>Attach to a transparent UI panel on the right side of the screen.</summary>
    public sealed class TouchLookPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private MobileInputBridge inputBridge;
        [SerializeField, Range(0.1f, 2f)] private float sensitivity = 0.65f;
        private Vector2 previousPosition;
        private int activePointer = int.MinValue;

        public void OnPointerDown(PointerEventData eventData)
        {
            activePointer = eventData.pointerId;
            previousPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointer || inputBridge == null) return;
            Vector2 delta = eventData.position - previousPosition;
            previousPosition = eventData.position;
            inputBridge.SetLookInput(delta * sensitivity);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointer) return;
            activePointer = int.MinValue;
            if (inputBridge != null) inputBridge.SetLookInput(Vector2.zero);
        }
    }
}