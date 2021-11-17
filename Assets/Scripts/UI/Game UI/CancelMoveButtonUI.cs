public class CancelMoveButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            if (currentUnit != null) {
                if (currentUnit.turnStage == TurnStage.PathSet) {
                    Enable();
                } else {
                    Disable();
                }
            } else {
                Disable();
            }
        } else {
            buttonParent.SetActive(false);

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
        if (currentUnit.turnStage == TurnStage.PathSet) {
            Enable();
        } else if (currentUnit.turnStage == TurnStage.Complete) {
            button.interactable = false;
        } else {
            Disable();
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        currentUnit.UnsetPath();

        if (currentUnit.moves > 0) {
            GameManager.Instance.GetCurrentPlayer().turnCompleted = false;
            GameManager.Instance.GetCurrentPlayer().unitQueue.Add(currentUnit);
            currentUnit.turnStage = TurnStage.Started;
        } else {
            currentUnit.turnStage = TurnStage.Complete;
        }
    }
}