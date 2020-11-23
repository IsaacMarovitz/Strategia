using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FailureUI : MonoBehaviour {
    public Button okButton;
    public GameObject failurePanel;
    public TMP_Text failureText;

    void Start() => okButton.onClick.AddListener(Ok);

    public void DisplayError(string error) {
        failureText.text = error;
        failurePanel.SetActive(true);
    }

    void Ok() => failurePanel.SetActive(false);
}
