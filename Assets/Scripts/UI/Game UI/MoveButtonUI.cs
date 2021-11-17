using UnityEngine;

public class MoveButtonUI : GameButtonUI {

    public bool unitIsMoving = false;
    public bool moveButtonPressed = false;

    public void Update() {
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            button.interactable = false;

            if (currentUnit != null) {
                if (currentUnit.turnStage == TurnStage.PathSet) {
                    buttonParent.SetActive(false);
                } else {
                    buttonParent.SetActive(true);
                }
            } else {
                buttonParent.SetActive(true);
            }
        } else {
            buttonParent.SetActive(true);

            if (currentUnit != null) {
                UnitMoveUI unitMoveUI = currentUnit.unitMoveUI;

                if (!button.interactable && currentUnit != gameUI.oldUnit) {
                    gameUI.oldUnit = currentUnit;
                    button.interactable = true;
                    unitMoveUI.MoveButtonDeselected();
                }

                // If the right mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, and set Move Button to interactable
                if (Input.GetMouseButtonUp(1) && unitMoveUI.isMoving && !gameUI.cameraController.didRMBDrag) {
                    unitMoveUI.MoveButtonDeselected();
                    button.interactable = true;
                    unitIsMoving = false;
                }

                // If the left mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, set Move Button to interactable, and move the selected Unit
                if (Input.GetMouseButtonUp(0) && unitMoveUI.isMoving && !gameUI.cameraController.didLMBDrag) {
                    if (gameUI.cameraController.IsMouseOverUI() && moveButtonPressed) {
                        if (!moveButtonPressed) {
                            unitMoveUI.MoveButtonDeselected();
                            button.interactable = true;
                            unitIsMoving = false;
                        }
                    } else {
                        if (unitMoveUI.Move()) {
                            unitMoveUI.MoveButtonDeselected();
                            button.interactable = true;
                            unitIsMoving = false;
                        }
                    }
                }
            } else {
                button.interactable = false;
            }

            if (!GameManager.Instance.dayCompleted) {
                if (currentUnit != null) {
                    UpdateUI();
                }
            } else {
                button.interactable = false;
            }
        }
    }

    public void UpdateUI() {
        if (currentUnit.turnStage == TurnStage.Sleeping) {
            button.interactable = false;
        } else if (currentUnit.turnStage == TurnStage.Complete) {
            button.interactable = false;
        } else if (currentUnit.turnStage == TurnStage.PathSet) {
            Disable();
        } else {
            buttonParent.SetActive(true);

            if (unitIsMoving) {
                button.interactable = false;
            } else {
                button.interactable = true;
            }
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        gameUI.oldUnit = currentUnit;
        currentUnit.unitMoveUI.MoveButtonSelected();
        button.interactable = false;
        unitIsMoving = true;
        moveButtonPressed = true;
    }
} 