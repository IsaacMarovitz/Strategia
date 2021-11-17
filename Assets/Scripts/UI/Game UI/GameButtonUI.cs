using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameButtonUI : MonoBehaviour {
    protected GameUI gameUI;
    protected GameObject buttonParent;
    protected Button button;
    protected TMP_Text buttonText;
    protected Unit currentUnit {
        get {
            if (gameUI != null) {
                return gameUI.currentUnit;
            } else {
                return null;
            }
        }
        set {
            if (gameUI != null) {
                gameUI.currentUnit = value;
            }
        }
    }

    public void Start() {
        gameUI = FindObjectOfType<GameUI>();
        buttonParent = transform.GetChild(0).gameObject;
        button = transform.GetComponentInChildren<Button>();
        buttonText = transform.GetComponentInChildren<TMP_Text>();

        button.onClick.AddListener(ButtonEvent);
    }

    public void Enable() {
        buttonParent.SetActive(true);
        button.interactable = true;
    }

    public void Disable() {
        buttonParent.SetActive(false);
        button.interactable = false;
    }

    public virtual void ButtonEvent() {}
}