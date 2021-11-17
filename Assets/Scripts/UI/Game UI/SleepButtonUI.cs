public class SleepButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (currentUnit != null) {
            if (currentUnit.unitTurnStage != UnitTurnStage.Sleeping && currentUnit.unitTurnStage != UnitTurnStage.PathSet) {
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