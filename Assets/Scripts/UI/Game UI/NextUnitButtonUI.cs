public class NextUnitButtonUI : GameButtonUI {
    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            buttonParent.SetActive(false);
        } else {
            buttonParent.SetActive(true);
        }
    }

    public override void ButtonEvent() {
        Unit newCurrentUnit = GameManager.Instance.GetCurrentPlayer().GetCurrentUnit();
        UIData.SetCity(null);

        if (newCurrentUnit != null) {
            if (currentUnit == null) {
                currentUnit = newCurrentUnit;
            } else {
                GameManager.Instance.GetCurrentPlayer().NextUnit(newCurrentUnit, true);
            }
            gameUI.cameraController.Focus(GridUtilities.TileToWorldPos(currentUnit.pos), true);
        }    
    }
}