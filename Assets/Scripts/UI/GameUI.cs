using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {

    [Header("Text")]
    public TMP_Text movesLeft;
    public TMP_Text fuelLeft;
    public TMP_Text dayCounter;
    public TMP_Text customButtonText;

    [Header("Buttons")]
    public Button moveButton;
    public Button cancelMoveButton;
    public Button sleepButton;
    public Button wakeButton;
    public Button customButton;
    public Button doneButton;
    public Button endTurnButton;
    public Button nextUnitButton;

    [Header("Button Parents")]
    public GameObject moveButtonParent;
    public GameObject cancelMoveButtonParent;
    public GameObject sleepButtonParent;
    public GameObject wakeButtonParent;
    public GameObject customButtonParent;
    public GameObject endTurnButtonParent;
    public GameObject nextUnitButtonParent;

    [Header("Misc")]
    public Image unitImage;
    public Slider healthSlider;
    public CameraController cameraController;

    private Unit oldUnit;
    private bool unitIsMoving = false;
    private bool moveButtonPressed = false;
    private Unit currentUnit {
        get {
            return UIData.currentUnit;
        }
        set {
            UIData.SetUnit(value);
        }
    }

    public void Start() {
        // Setup onClick events for all main bottom bar buttons
        moveButton.onClick.AddListener(MoveButton);
        cancelMoveButton.onClick.AddListener(CancelMoveButton);
        sleepButton.onClick.AddListener(SleepButton);
        wakeButton.onClick.AddListener(WakeButton);
        customButton.onClick.AddListener(CustomButton);
        doneButton.onClick.AddListener(DoneButton);
        endTurnButton.onClick.AddListener(EndTurnButton);
        nextUnitButton.onClick.AddListener(NextUnitButton);

        oldUnit = currentUnit;
    }

    public void Update() {
        dayCounter.text = $"Day {GameManager.Instance.day}";
        unitImage.color = GameManager.Instance.GetCurrentPlayer().playerColor;

        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            sleepButtonParent.SetActive(true);
            customButtonParent.SetActive(false);
            endTurnButtonParent.SetActive(true);
            nextUnitButtonParent.SetActive(false);

            moveButton.interactable = false;
            doneButton.interactable = false;

            movesLeft.text = "";
            fuelLeft.text = "";
            oldUnit = null;

            if (currentUnit != null) {
                if (currentUnit.unitIcon != null) {
                    unitImage.sprite = currentUnit.unitIcon;
                }

                if (currentUnit.turnStage == TurnStage.PathSet) {
                    cancelMoveButtonParent.SetActive(true);
                    moveButtonParent.SetActive(false);
                    sleepButtonParent.SetActive(true);
                    wakeButtonParent.SetActive(false);

                    sleepButton.interactable = false;
                    cancelMoveButton.interactable = true;
                } else if (currentUnit.turnStage == TurnStage.Sleeping) {
                    wakeButtonParent.SetActive(true);
                    sleepButtonParent.SetActive(false);

                    wakeButton.interactable = true;
                } else {
                    moveButtonParent.SetActive(true);
                    cancelMoveButtonParent.SetActive(false);
                    sleepButtonParent.SetActive(true);
                    wakeButtonParent.SetActive(false);

                    cancelMoveButton.interactable = false;
                    sleepButton.interactable = true;
                }
            } else {
                moveButtonParent.SetActive(true);
                cancelMoveButtonParent.SetActive(false);

                cancelMoveButton.interactable = false;
            }
        } else {
            moveButtonParent.SetActive(true);
            cancelMoveButtonParent.SetActive(false);
            endTurnButtonParent.SetActive(false);
            nextUnitButtonParent.SetActive(true);

            if (currentUnit != null) {
                UnitMoveUI unitMoveUI = currentUnit.unitMoveUI;

                healthSlider.maxValue = currentUnit.maxHealth;
                healthSlider.value = currentUnit.health;
                if (currentUnit.unitIcon != null) {
                    unitImage.sprite = currentUnit.unitIcon;
                }

                movesLeft.text = $"Moves Left: {currentUnit.moves}";

                // If the unit inherits the ICustomButton interface, activate CustomButton and set CustomButtonText to CustomButtonName
                ICustomButton buttonInterface = currentUnit as ICustomButton;
                if (buttonInterface != null) {
                    customButtonText.text = buttonInterface.CustomButtonName;
                    customButtonParent.SetActive(true);
                } else {
                    customButtonParent.SetActive(false);
                }

                // If the unit inherits the IFuel interface, set FuelLeft tect to the current fuel level
                IFuel fuelInterface = currentUnit as IFuel;
                if (fuelInterface != null) {
                    fuelLeft.text = $"Fuel: {fuelInterface.fuel}";
                } else {
                    fuelLeft.text = "";
                }

                // If a different unit has been selected, disable the UnitMoveUI Line Renderer, and set MoveButton to interactable 
                if (!moveButton.interactable && currentUnit != oldUnit) {
                    oldUnit = currentUnit;
                    moveButton.interactable = true;
                    unitMoveUI.MoveButtonDeselected();
                }

                // If the right mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, and set Move Button to interactable
                if (Input.GetMouseButtonUp(1) && unitMoveUI.isMoving && !cameraController.didRMBDrag) {
                    unitMoveUI.MoveButtonDeselected();
                    moveButton.interactable = true;
                    unitIsMoving = false;
                }

                // If the left mouse button is pressed while the line is showing, disable the UnitMoveUI Line Renderer, set Move Button to interactable, and move the selected Unit
                if (Input.GetMouseButtonUp(0) && unitMoveUI.isMoving && !cameraController.didLMBDrag) {
                    if (cameraController.IsMouseOverUI() && moveButtonPressed) {
                        if (!moveButtonPressed) {
                            unitMoveUI.MoveButtonDeselected();
                            moveButton.interactable = true;
                            unitIsMoving = false;
                        }
                    } else {
                        if (unitMoveUI.Move()) {
                            unitMoveUI.MoveButtonDeselected();
                            moveButton.interactable = true;
                            unitIsMoving = false;
                        }
                    }
                }
            } else {
                customButtonParent.SetActive(false);
                endTurnButtonParent.SetActive(false);
                nextUnitButtonParent.SetActive(true);
                movesLeft.text = "";
                fuelLeft.text = "";
                SetButtons(false);

                oldUnit = null;
            }

            if (!GameManager.Instance.dayCompleted) {
                if (currentUnit != null)
                    UpdateUI();
            } else {
                SetButtons(false);
            }
        }
    }

    public void UpdateUI() {
        if (currentUnit.turnStage == TurnStage.Waiting) {
            currentUnit.StartTurn();
        } else if (currentUnit.turnStage == TurnStage.Sleeping) {
            sleepButtonParent.SetActive(false);
            wakeButtonParent.SetActive(true);
            moveButton.interactable = false;
            sleepButton.interactable = false;
            doneButton.interactable = false;
            wakeButton.interactable = true;
        } else if (currentUnit.turnStage == TurnStage.Complete) {
            SetButtons(false);
        } else if (currentUnit.turnStage == TurnStage.PathSet) {
            cancelMoveButtonParent.SetActive(true);
            moveButtonParent.SetActive(false);
            cancelMoveButton.interactable = true;
            moveButton.interactable = false;
        } else {
            moveButtonParent.SetActive(true);
            cancelMoveButtonParent.SetActive(false);
            sleepButtonParent.SetActive(true);
            wakeButtonParent.SetActive(false);
            moveButton.interactable = true;
            cancelMoveButton.interactable = false;
            sleepButton.interactable = true;
            wakeButton.interactable = true;
            doneButton.interactable = true;
            if (unitIsMoving) {
                moveButton.interactable = false;
            } else {
                moveButton.interactable = true;
            }
        }
    }

    public void SetButtons(bool isActive) {
        moveButton.interactable = isActive;
        cancelMoveButton.interactable = isActive;
        sleepButton.interactable = isActive;
        wakeButton.interactable = isActive;
        doneButton.interactable = isActive;
    }

    #region  Button Functions

    public void MoveButton() {
        if (currentUnit == null) { return; }

        oldUnit = currentUnit;
        currentUnit.unitMoveUI.MoveButtonSelected();
        moveButton.interactable = false;
        unitIsMoving = true;
        moveButtonPressed = true;
    }

    public void CancelMoveButton() {
        if (currentUnit == null) { return; }

        currentUnit.UnsetPath();

        if (currentUnit.moves > 0) {
            GameManager.Instance.GetCurrentPlayer().turnCompleted = false;
            GameManager.Instance.GetCurrentPlayer().unitQueue.Add(currentUnit);
            currentUnit.turnStage = TurnStage.Started;
        } else {
            currentUnit.turnStage = TurnStage.Complete;
        }
    }

    public void SleepButton() {
        if (currentUnit == null) { return; }

        currentUnit.ToggleSleep();
    }

    public void WakeButton() {
        if (currentUnit == null) { return; }

        currentUnit.ToggleSleep();
    }

    public void DoneButton() {
        if (currentUnit == null) { return; }

        currentUnit.EndTurn();
    }

    public void CustomButton() {
        if (currentUnit == null) { return; }

        ICustomButton currentUnitInterface = currentUnit as ICustomButton;
        if (currentUnitInterface != null) {
            currentUnitInterface.CustomButton();
        }
    }

    public void EndTurnButton() {
        GameManager.Instance.GetCurrentPlayer().TurnComplete();
    }

    public void NextUnitButton() {
        Unit newCurrentUnit = GameManager.Instance.GetCurrentPlayer().GetCurrentUnit();
        UIData.SetCity(null);

        if (newCurrentUnit != null) {
            if (currentUnit == null) {
                currentUnit = newCurrentUnit;
            } else {
                GameManager.Instance.GetCurrentPlayer().NextUnit(newCurrentUnit, true);
            }
            cameraController.Focus(GridUtilities.TileToWorldPos(currentUnit.pos), true);
        }
    }

    #endregion
}