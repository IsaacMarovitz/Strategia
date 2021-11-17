using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameButtonUI : TurnBehaviour {
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

        button.onClick.AddListener(ButtonEventInvoke);
    }

    public override void OnPlayerTurnWait(Player player) {
        UpdateUI();
    }

    public override void OnPlayerTurnStart(Player player) {
        UpdateUI();
    }

    public override void OnPlayerTurnComplete(Player player) {
        UpdateUI();
    }

    public override void OnPlayerTurnEnd(Player player) {
        UpdateUI();
    }

    public override void OnUnitAction() {
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