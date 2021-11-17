public class CancelMoveButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (currentUnit != null) {
            if (currentUnit.unitTurnStage == UnitTurnStage.PathSet) {
                Enable();
            } else {
                Disable();
            }
        } else {
            Disable();
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        currentUnit.UnsetPath();

        if (currentUnit.moves > 0) {
            GameManager.Instance.GetCurrentPlayer().playerTurnStage = PlayerTurnStage.Started;
            GameManager.Instance.GetCurrentPlayer().unitQueue.Add(currentUnit);
            currentUnit.unitTurnStage = UnitTurnStage.Started;
        } else {
            currentUnit.unitTurnStage = UnitTurnStage.Complete;
        }
    }
}