using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler {

    public TabGroup tabGroup;
    public Image background;
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    public void OnPointerClick(PointerEventData eventData) {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tabGroup.OnTabExit(this);
    }

    void Start() {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void Select() {
        onTabSelected?.Invoke();
    }

    public void Deselect() {
        onTabDeselected?.Invoke();
    }
}
