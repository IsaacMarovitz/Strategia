using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private RectTransform canvasRectTransform = null;
    [SerializeField] private RectTransform dragRectTransform = null;
    [SerializeField] private RectTransform panelRectTransform = null;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private float fadeAlpha = 0.4f;
    [SerializeField] private float fadeDuration = 0.1f;

    private void Start() {
        dragRectTransform.sizeDelta = new Vector2(panelRectTransform.rect.width / 2, panelRectTransform.rect.height / 2);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Open(Action onCompleteDelegate) {
        canvasGroup.DOFade(1f, fadeDuration).OnComplete(() => {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            onCompleteDelegate();
        });
    }

    public void Close(Action onCompleteDelegate) {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => {
            onCompleteDelegate();
        });
    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.DOFade(fadeAlpha, fadeDuration);
    }

    public void OnDrag(PointerEventData eventData) {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.DOFade(1f, fadeDuration);

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
