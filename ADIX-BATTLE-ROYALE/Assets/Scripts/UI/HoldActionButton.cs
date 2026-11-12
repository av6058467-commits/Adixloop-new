using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AdixBattleRoyale.UI
{
    // Attach to a Unity UI Button/Image with a raycast target enabled.
    // Wire actions in the Inspector; visual states are controlled by the UI Animator.
    public sealed class HoldActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent onPressed;
        [SerializeField] private UnityEvent onReleased;
        [SerializeField] private bool repeatWhileHeld;
        [SerializeField, Min(0.05f)] private float repeatInterval = 0.12f;

        private bool held;
        private float nextRepeat;

        public bool IsHeld => held;

        public void OnPointerDown(PointerEventData eventData)
        {
            held = true;
            nextRepeat = Time.unscaledTime + repeatInterval;
            onPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData) => Release();
        public void OnPointerExit(PointerEventData eventData) => Release();

        private void Update()
        {
            if (!held || !repeatWhileHeld || Time.unscaledTime < nextRepeat) return;
            nextRepeat = Time.unscaledTime + repeatInterval;
            onPressed?.Invoke();
        }

        private void Release()
        {
            if (!held) return;
            held = false;
            onReleased?.Invoke();
        }

        private void OnDisable() => Release();
    }
}
