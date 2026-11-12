using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Attach to a UI joystick background. Assign the handle RectTransform.</summary>
public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform area;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(20f, 200f)] private float radius = 90f;
    public Vector2 Value { get; private set; }

    private void Awake()
    {
        if (area == null) area = transform as RectTransform;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (area == null) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            area, eventData.position, eventData.pressEventCamera, out var local)) return;

        var normalized = Vector2.ClampMagnitude(local / Mathf.Max(1f, radius), 1f);
        Value = normalized;
        if (handle != null) handle.anchoredPosition = normalized * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}