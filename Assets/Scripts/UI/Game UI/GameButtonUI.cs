using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameButtonUI : TurnBehaviour {
    protected GameUI gameUI;
    public GameObject buttonParent;
    public Button button;
    public TMP_Text buttonText;
    protected Unit currentUnit {
        get {
            return UIData.currentUnit;
        }
        set {
            UIData.SetUnit(value);
        }
    }

    public void Start() {
        gameUI = FindObjectOfType<GameUI>();

        if (button != null) {
            button.onClick.AddListener(ButtonEventInvoke);
        }
    }

    public override void OnPlayerStateChanged(Player player) {
        UpdateUI();
    }

    public override void OnUnitAction() {
        UpdateUI();
    }

    public override void OnUnitTurnStart(Unit unit) {
        UpdateUI();
    }

    public override void OnUnitSelected(Unit unit) {
        UpdateUI();
    }

    public override void OnUnitDeselected() {
        UpdateUI();
    }

    public void Enable() {
        buttonParent.SetActive(true);
        button.interactable = true;
    }

    public void Disable() {
        buttonParent.SetActive(false);
        button.interactable = false;
    }

    public void ButtonEventInvoke() {
        ButtonEvent();
        DelegateManager.unitActionDelegate?.Invoke();
    }

    public virtual void ButtonEvent() {}

    public virtual void UpdateUI() {}
}