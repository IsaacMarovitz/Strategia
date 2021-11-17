public class DoneButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            button.interactable = false;
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
            button.interactable = false;
        } else if (currentUnit.turnStage == TurnStage.Complete) {
            button.interactable = false;
        } else {
            button.interactable = true;
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        currentUnit.EndTurn();
    }
}