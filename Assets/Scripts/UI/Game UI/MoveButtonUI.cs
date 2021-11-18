using UnityEngine;

public class MoveButtonUI : GameButtonUI {

    public bool unitIsMoving = false;
    public bool moveButtonPressed = false;
    public bool mouseOnePressed = false;
    public bool mouseZeroPressed = false;

    public void Update() {
        if (Input.GetMouseButtonUp(1)) {
            mouseOnePressed = true;
            UpdateUI();
            mouseOnePressed = false;
        }

        if (Input.GetMouseButtonUp(0)) {
            mouseZeroPressed = true;
            UpdateUI();
            mouseZeroPressed = false;
        }
    }

    public override void UpdateUI() {
        if (currentUnit == null) {
            Disable();
            return; 
        }

        if (GameManager.Instance.GetCurrentPlayer().playerTurnStage != PlayerTurnStage.Complete) {
            if (currentUnit.unitTurnStage == UnitTurnStage.Started) {
                Enable();
                UnitMoveUI unitMoveUI = currentUnit.unitMoveUI;
                if (!button.interactable && currentUnit != gameUI.oldUnit) {
                    gameUI.oldUnit = currentUnit;
                    button.interactable = true;
                    unitMoveUI.MoveButtonDeselected();
                }

                // If the right mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, and set Move Button to interactable
                if (mouseOnePressed && unitMoveUI.isMoving && !gameUI.cameraController.didRMBDrag) {
                    unitMoveUI.MoveButtonDeselected();
                    button.interactable = true;
                    unitIsMoving = false;
                }

                // If the left mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, set Move Button to interactable, and move the selected Unit
                if (mouseZeroPressed && unitMoveUI.isMoving && !gameUI.cameraController.didLMBDrag) {
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

                if (unitIsMoving) {
                    button.interactable = false;
                } else {
                    button.interactable = true;
                }
            } else {
                Disable();
            }
        } else {
            Disable();
        }

        if (currentUnit.unitTurnStage == UnitTurnStage.PathSet) {
            if (currentUnit.moves > 0) {
                Enable();
            }
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        if (currentUnit.unitTurnStage == UnitTurnStage.Started) {
            gameUI.oldUnit = currentUnit;
            currentUnit.unitMoveUI.MoveButtonSelected();
            button.interactable = false;
            unitIsMoving = true;
            moveButtonPressed = true;
        } else if (currentUnit.unitTurnStage == UnitTurnStage.PathSet) {
            currentUnit.MoveAlongSetPath();
        }
    }
} 