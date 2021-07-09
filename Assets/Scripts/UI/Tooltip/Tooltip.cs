using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour {

    public RectTransform canvasRectTransform;
    public RectTransform backgroundRectTransform;
    public RectTransform rectTransform;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;
    public LayoutElement layoutElement;
    public GameObject image;
    public int characterWrapLimit;

    public void SetText(string content, string header = "") {
        if (string.IsNullOrEmpty(header)) {
            headerText.gameObject.SetActive(false);
        } else {
            headerText.gameObject.SetActive(true);
            headerText.text = header;
        }

        contentText.text = content;

        int headerLength = headerText.text.Length;
        int contentLenght = contentText.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit) || (contentLenght > characterWrapLimit) ? true : false;
    }

    public void Update() {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width) {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }

        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height) {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }

        if (anchoredPosition.x < 0) {
            anchoredPosition.x = 0;
        }

        if (anchoredPosition.y < 0) {
            anchoredPosition.y = 0;
        }

        rectTransform.anchoredPosition = anchoredPosition;
    }
}
