using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private RectTransform dragRectTransform = null;
    [SerializeField] private Image backgroundImage = null;
    private Color backgroundColor;

    private void Awake() {
        backgroundColor = backgroundImage.color;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        backgroundColor.a = 0.4f;
        backgroundImage.color = backgroundColor;
    }

    public void OnDrag(PointerEventData eventData) {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        backgroundColor.a = 1f;
        backgroundImage.color = backgroundColor;
    }
}
