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
    public MoveUI moveUI;
    public CancelMoveUI cancelMoveUI;
    public CameraController cameraController;

    private Unit oldUnit;
    private bool unitIsMoving = false;
    private bool moveButtonPressed = false;

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

        oldUnit = UIData.Instance.currentUnit;
    }

    public void Update() {
        dayCounter.text = $"Day {GameManager.Instance.day}";
        unitImage.color = GameManager.Instance.GetCurrentPlayer().playerColor;

        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            sleepButtonParent.SetActive(true);
            wakeButtonParent.SetActive(false);
            customButtonParent.SetActive(false);
            endTurnButtonParent.SetActive(true);
            nextUnitButtonParent.SetActive(false);

            moveButton.interactable = false;
            sleepButton.interactable = false;
            wakeButton.interactable = false;
            doneButton.interactable = false;
            
            movesLeft.text = "";
            fuelLeft.text = "";
            oldUnit = null;

            if (UIData.Instance.currentUnit != null) {
                if (UIData.Instance.currentUnit.unitIcon != null) {
                    unitImage.sprite = UIData.Instance.currentUnit.unitIcon;
                }

                if (UIData.Instance.currentUnit.turnStage == TurnStage.PathSet) {
                    cancelMoveButtonParent.SetActive(true);
                    moveButtonParent.SetActive(false);

                    cancelMoveButton.interactable = true;
                } else {
                    moveButtonParent.SetActive(true);
                    cancelMoveButtonParent.SetActive(false);

                    cancelMoveButton.interactable = false;
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

            if (UIData.Instance.currentUnit != null) {
                healthSlider.maxValue = UIData.Instance.currentUnit.maxHealth;
                healthSlider.value = UIData.Instance.currentUnit.health;
                if (UIData.Instance.currentUnit.unitIcon != null) {
                    unitImage.sprite = UIData.Instance.currentUnit.unitIcon;
                }
                
                movesLeft.text = $"Moves Left: {UIData.Instance.currentUnit.moves}";

                // If the unit inherits the ICustomButton interface, activate CustomButton and set CustomButtonText to CustomButtonName
                ICustomButton buttonInterface = UIData.Instance.currentUnit as ICustomButton;
                if (buttonInterface != null) {
                    customButtonText.text = buttonInterface.CustomButtonName;
                    customButtonParent.SetActive(true);
                } else {
                    customButtonParent.SetActive(false);
                }

                // If the unit inherits the IFuel interface, set FuelLeft tect to the current fuel level
                IFuel fuelInterface = UIData.Instance.currentUnit as IFuel;
                if (fuelInterface != null) {
                    fuelLeft.text = $"Fuel: {fuelInterface.fuel}";
                } else {
                    fuelLeft.text = "";
                }

                // If a different unit has been selected, disable the MoveUI Line Renderer, and set MoveButton to interactable 
                if (!moveButton.interactable && UIData.Instance.currentUnit != oldUnit) {
                    oldUnit = UIData.Instance.currentUnit;
                    moveButton.interactable = true;
                    moveUI.Hide();
                }

                // If the right mouse button is pressed while the line is showing, disable the MoveUI Line Renderer, and set Move Button to interactable
                if (Input.GetMouseButtonUp(1) && moveUI.showLine && !cameraController.didRMBDrag) {
                    moveUI.Hide();
                    moveButton.interactable = true;
                    unitIsMoving = false;
                }

                // If the left mouse button is pressed while the line is showing, disable the MoveUI Line Renderer, set Move Button to interactable, and move the selected Unit
                if (Input.GetMouseButtonUp(0) && moveUI.showLine && !cameraController.didLMBDrag) {
                    if (cameraController.IsMouseOverUI() && moveButtonPressed) {
                        if (!moveButtonPressed) {
                            moveUI.Hide();
                            moveButton.interactable = true;
                            unitIsMoving = false;
                        }
                    } else {
                        moveUI.Hide();
                        moveButton.interactable = true;
                        unitIsMoving = false;
                        moveUI.Move();
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
                moveUI.path = null;
            }

            if (!GameManager.Instance.dayCompleted) {
                if (UIData.Instance.currentUnit != null)
                    UpdateUI();
            } else {
                SetButtons(false);
            }
        }
    }

    public void UpdateUI() {
        if (UIData.Instance.currentUnit.turnStage == TurnStage.Waiting) {
            UIData.Instance.currentUnit.StartTurn();
            moveUI.Hide();
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.Sleeping) {
            sleepButtonParent.SetActive(false);
            wakeButtonParent.SetActive(true);
            moveButton.interactable = false;
            sleepButton.interactable = false;
            doneButton.interactable = false;
            wakeButton.interactable = true;
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.Complete) {
            SetButtons(false);
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.PathSet) {
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
        if (UIData.Instance.currentUnit == null) { return; }

        oldUnit = UIData.Instance.currentUnit;
        moveUI.Show();
        moveButton.interactable = false;
        unitIsMoving = true;
        moveButtonPressed = true;
    }

    public void CancelMoveButton() {
        if (UIData.Instance.currentUnit == null) { return; }

        UIData.Instance.currentUnit.UnsetPath();

        if (UIData.Instance.currentUnit.moves > 0) {
            GameManager.Instance.GetCurrentPlayer().turnCompleted = false;
            GameManager.Instance.GetCurrentPlayer().unitQueue.Add(UIData.Instance.currentUnit);
            UIData.Instance.currentUnit.turnStage = TurnStage.Started;
        } else {
            UIData.Instance.currentUnit.turnStage = TurnStage.Complete;
        }
    }

    public void SleepButton() {
        if (UIData.Instance.currentUnit == null) { return; }

        UIData.Instance.currentUnit.ToggleSleep();
    }

    public void WakeButton() {
        if (UIData.Instance.currentUnit == null) { return; }

        UIData.Instance.currentUnit.ToggleSleep();
    }

    public void DoneButton() {
        if (UIData.Instance.currentUnit == null) { return; }

        UIData.Instance.currentUnit.EndTurn();
    }

    public void CustomButton() {
        if (UIData.Instance.currentUnit == null) { return; }

        ICustomButton currentUnitInterface = UIData.Instance.currentUnit as ICustomButton;
        if (currentUnitInterface != null) {
            currentUnitInterface.CustomButton();
        }
    }

    public void EndTurnButton() {
        GameManager.Instance.GetCurrentPlayer().TurnComplete();
    }

    public void NextUnitButton() {
        Unit currentUnit = GameManager.Instance.GetCurrentPlayer().GetCurrentUnit();
        UIData.Instance.currentCity = null;

        if (currentUnit != null) {
            if (UIData.Instance.currentUnit == null) {
                UIData.Instance.currentUnit = currentUnit;
            } else {
                GameManager.Instance.GetCurrentPlayer().NextUnit(currentUnit, true);
            }
            cameraController.Focus(GridUtilities.TileToWorldPos(UIData.Instance.currentUnit.pos), true);
        }
    }

    #endregion
}