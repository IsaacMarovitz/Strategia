public class WakeButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (currentUnit != null) {
            if (currentUnit.unitTurnStage == UnitTurnStage.Sleeping) {
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

        currentUnit.ToggleSleep();
    }
}