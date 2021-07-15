using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private RectTransform canvasRectTransform = null;
    [SerializeField] private RectTransform dragRectTransform = null;
    [SerializeField] private RectTransform panelRectTransform = null;
    [SerializeField] private Image backgroundImage = null;
    private Color backgroundColor;

    private void Awake() {
        backgroundColor = backgroundImage.color;
    }

    private void Start() {
        dragRectTransform.sizeDelta = new Vector2(panelRectTransform.rect.width / 2, panelRectTransform.rect.height / 2);
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

        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        Vector2 anchoredPosition = dragRectTransform.anchoredPosition;
        if (anchoredPosition.x + dragRectTransform.rect.width > canvasRectTransform.rect.width / 2) {
            anchoredPosition.x = canvasRectTransform.rect.width / 2 - dragRectTransform.rect.width;
        }
        if (anchoredPosition.y + dragRectTransform.rect.height > canvasRectTransform.rect.height / 2) {
            anchoredPosition.y = canvasRectTransform.rect.height / 2 - dragRectTransform.rect.height;
        }
        if (-(anchoredPosition.x - dragRectTransform.rect.width) > canvasRectTransform.rect.width / 2) {
            anchoredPosition.x = -(canvasRectTransform.rect.width / 2) + dragRectTransform.rect.width;
        }
        if (-(anchoredPosition.y - dragRectTransform.rect.height) > canvasRectTransform.rect.height / 2) {
            anchoredPosition.y = -(canvasRectTransform.rect.height / 2) + dragRectTransform.rect.height;
        }
        dragRectTransform.anchoredPosition = anchoredPosition;
    }
}
