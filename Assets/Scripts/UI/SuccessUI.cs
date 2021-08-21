using UnityEngine;
using UnityEngine.UI;

public class SuccessUI : MonoBehaviour {
    public Button okButton;
    public DragWindow successDragWindow;

    void Start() => okButton.onClick.AddListener(Ok);

    void Ok() => successDragWindow.Close(() => {});
}
