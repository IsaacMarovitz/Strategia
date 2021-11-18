public class DoneButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (GameManager.Instance.GetCurrentPlayer().playerTurnStage != PlayerTurnStage.Complete) {
            if (currentUnit != null) {
                if (currentUnit.unitTurnStage == UnitTurnStage.Started) {
                    Enable();
                } else {
                    Disable();
                }
            } else {
                Disable();
            }
        } else {
            Disable();
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }
        
        currentUnit.unitTurnStage = UnitTurnStage.Complete;
        currentUnit.EndTurn();
    }
}