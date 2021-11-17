public class CustomButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            buttonParent.SetActive(false);
        } else {
            if (currentUnit != null) {
                ICustomButton buttonInterface = currentUnit as ICustomButton;
                if (buttonInterface != null) {
                    buttonText.text = buttonInterface.CustomButtonName;
                    buttonParent.SetActive(true);
                } else {
                    buttonParent.SetActive(false);
                }
            } else {
                buttonParent.SetActive(false);
            }
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