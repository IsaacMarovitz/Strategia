using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FailureUI : MonoBehaviour {
    public Button okButton;
    public DragWindow failureDragWindow;
    public TMP_Text failureText;

    void Start() => okButton.onClick.AddListener(Ok);

    public void DisplayError(string error) {
        failureText.text = error;
        failureDragWindow.Open(() => {});
    }

    void Ok() => failureDragWindow.Close(() => {});
}
