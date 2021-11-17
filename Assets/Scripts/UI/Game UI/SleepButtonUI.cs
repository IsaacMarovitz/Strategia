public class SleepButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            buttonParent.SetActive(true);

            if (currentUnit != null) {
                if (currentUnit.turnStage == TurnStage.PathSet) {
                    buttonParent.SetActive(true);
                    button.interactable = false;
                } else if (currentUnit.turnStage == TurnStage.Sleeping) {
                    buttonParent.SetActive(false);
                } else {
                    Enable();
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
            Disable();
        } else if (currentUnit.turnStage == TurnStage.Complete) {
            button.interactable = false;
        } else {
            Enable();
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        currentUnit.ToggleSleep();
    }
}