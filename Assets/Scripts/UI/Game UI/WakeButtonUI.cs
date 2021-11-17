public class WakeButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            buttonParent.SetActive(true);

            if (currentUnit != null) {
                if (currentUnit.turnStage == TurnStage.PathSet) {
                    buttonParent.SetActive(false);
                } else if (currentUnit.turnStage == TurnStage.Sleeping) {
                    Enable();
                } else {
                    buttonParent.SetActive(false);
                }
            }
        } else {
            if (currentUnit == null) {
                button.interactable = false;
            }

            if (!GameManager.Instance.dayCompleted) {
                if (currentUnit != null) {
                    UpdateUI();
                }
            } else {
                button.interactable = false;
            }
        }
    }

    public void UpdateUI() {
        if (currentUnit.turnStage == TurnStage.Sleeping) {
            Enable();
        } else if (currentUnit.turnStage == TurnStage.Complete) {
            button.interactable = false;
        } else {
            buttonParent.SetActive(false);
            button.interactable = true;
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        currentUnit.ToggleSleep();
    }
}