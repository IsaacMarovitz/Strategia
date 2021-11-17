public class EndTurnButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (GameManager.Instance.GetCurrentPlayer().playerTurnStage == PlayerTurnStage.Complete) {
            buttonParent.SetActive(true);
        } else {
            buttonParent.SetActive(false);
        }
    }

    public override void ButtonEvent() {
        GameManager.Instance.GetCurrentPlayer().TurnEnded();
    }
}