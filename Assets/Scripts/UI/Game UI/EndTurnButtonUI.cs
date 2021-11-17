public class EndTurnButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            buttonParent.SetActive(true);
        } else {
            buttonParent.SetActive(false);
        }
    }

    public override void ButtonEvent() {
        GameManager.Instance.GetCurrentPlayer().TurnComplete();
    }
}