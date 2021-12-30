using UnityEngine;

public class MoveButtonUI : GameButtonUI {

    public bool rightMousePressed = false;
    public bool leftMousePressed = false;
    public UnitMoveUI unitMoveUI;

    public void Update() {
        if (Input.GetMouseButtonUp(1)) {
            rightMousePressed = true;
            UpdateUI();
            rightMousePressed = false;
        }

        if (Input.GetMouseButtonUp(0)) {
            leftMousePressed = true;
            UpdateUI();
            leftMousePressed = false;
        }
    }

    public override void UpdateUI() {
        if (currentUnit == null) {
            unitMoveUI = null;
            Disable();
            return;
        }

        switch (currentUnit.unitTurnStage) {
            case UnitTurnStage.Started:
                if (GameManager.Instance.GetCurrentPlayer().playerTurnStage == PlayerTurnStage.Complete) {
                    Disable();
                    return;
                }

                Enable();
                unitMoveUI = currentUnit.unitMoveUI;

                if (unitMoveUI.isMoving) {
                    button.interactable = false;

                    // If the right mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, and set Move Button to interactable
                    if (rightMousePressed && !gameUI.cameraController.didRMBDrag && !gameUI.cameraController.IsMouseOverUI()) {
                        unitMoveUI.isMoving = false;
                        button.interactable = true;
                    }

                    // If the left mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, set Move Button to interactable, and move the selected Unit
                    if (leftMousePressed && !gameUI.cameraController.didLMBDrag && !gameUI.cameraController.IsMouseOverUI()) {
                        if (unitMoveUI.Move()) {
                            unitMoveUI.isMoving = false;
                            button.interactable = true;
                        }
                    }
                } else {
                    button.interactable = true;
                }
                break;
            case UnitTurnStage.PathSet:
                if (currentUnit.moves > 0) {
                    Enable();
                } else {
                    Disable();
                }
                break;
            default:
                Disable();
                break;
        }
    }

    public override void ButtonEvent() {
        if (currentUnit == null) { return; }

        if (currentUnit.unitTurnStage == UnitTurnStage.Started) {
            currentUnit.unitMoveUI.isMoving = true;
            button.interactable = false;
        } else if (currentUnit.unitTurnStage == UnitTurnStage.PathSet) {
            currentUnit.MoveAlongSetPath();
        }
    }
}