public class CustomButtonUI : GameButtonUI {
    public override void UpdateUI() {
        if (GameManager.Instance.GetCurrentPlayer().playerTurnStage != PlayerTurnStage.Complete) {
            if (currentUnit != null) {
                ICustomButton buttonInterface = currentUnit as ICustomButton;
                if (buttonInterface != null) {
                    buttonText.text = buttonInterface.CustomButtonName;
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

        ICustomButton currentUnitInterface = currentUnit as ICustomButton;
        if (currentUnitInterface != null) {
            currentUnitInterface.CustomButton();
        }
    }
}