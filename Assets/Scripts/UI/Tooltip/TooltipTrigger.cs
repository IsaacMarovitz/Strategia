using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string content;
    public string header;
    public float showDelay = 0.5f;
    public bool isEnabled;

    bool show = false;

    public void Awake() {
        isEnabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (isEnabled) {
            show = true;
            StartCoroutine(DelayedShow());
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        show = false;
        TooltipSystem.Hide();
    }

    public void OnMouseEnter() {
        if (isEnabled) {
            show = true;
            StartCoroutine(DelayedShow());
        }
    }

    public void OnMouseExit() {
        show = false;
        TooltipSystem.Hide();
    }

    IEnumerator DelayedShow() {
        yield return new WaitForSeconds(showDelay);
        if (show) {
            TooltipSystem.Show(content, header);
        }
    }
}
