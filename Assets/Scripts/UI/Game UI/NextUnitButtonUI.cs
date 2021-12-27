public class NextUnitButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (GameManager.Instance.GetCurrentPlayer().playerTurnStage == PlayerTurnStage.Complete) {
            buttonParent.SetActive(false);
        } else {
            buttonParent.SetActive(true);
        }
    }

    public override void ButtonEvent() {
        Unit newCurrentUnit = GameManager.Instance.GetCurrentPlayer().GetCurrentUnit();
        UIData.SetCity(null);

        GameManager.Instance.GetCurrentPlayer().NextUnit(newCurrentUnit, true);
    }
}