using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour {
    
    [Header("Text")]
    public TMP_Text movesLeft;
    public TMP_Text fuelLeft;
    public TMP_Text dayCounter;
    public TMP_Text newDayUIText;
    public TMP_Text customButtonText;
    public TMP_Text nextPlayerText;

    [Header("Buttons")]
    public Button moveButton;
    public Button sleepButton;
    public Button wakeButton;
    public Button doneButton;
    public Button customButton;
    public Button endTurnButton;
    public Button nextUnitButton;
    public Button nextPlayerButton;

    [Header("Button Parents")]
    public GameObject sleepButtonParent;
    public GameObject wakeButtonParent;
    public GameObject customButtonParent;
    public GameObject endTurnButtonParent;
    public GameObject nextUnitButtonParent;

    [Header("Misc")]
    public Image unitImage;
    public Slider healthSlider;
    public GameObject newDayUI;
    public float newDayWaitTime;
    public GameObject nextPlayerUI;
    public UnitUI unitUI;
    public CameraController cameraController;

    private Unit oldUnit;
    private bool nextPlayerUIEnabled = false;
    private bool unitIsMoving = false;

    public void Start() {
        // Setup onClick events for all main bottom bar buttons
        moveButton.onClick.AddListener(MoveButton);
        sleepButton.onClick.AddListener(SleepButton);
        wakeButton.onClick.AddListener(WakeButton);
        doneButton.onClick.AddListener(DoneButton);
        customButton.onClick.AddListener(CustomButton);
        endTurnButton.onClick.AddListener(EndTurnButton);
        nextUnitButton.onClick.AddListener(NextUnitButton);
        nextPlayerButton.onClick.AddListener(NextPlayerButton);

        // Subscribe UI functions to their appropriate GameManger delegates
        GameManager.Instance.newDayDelegate += NewDay;
        GameManager.Instance.nextPlayerDelegate += NextPlayer;

        oldUnit = UIData.Instance.currentUnit;
    }

    public void Update() {
        dayCounter.text = $"Day {GameManager.Instance.day}";
        newDayUIText.text = $"Day {GameManager.Instance.day + 1}";
        unitImage.color = GameManager.Instance.GetCurrentPlayer().playerColor;

        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            sleepButtonParent.SetActive(true);
            wakeButtonParent.SetActive(false);
            customButtonParent.SetActive(false);
            endTurnButtonParent.SetActive(true);
            nextUnitButtonParent.SetActive(false);
            SetButtons(false);

            movesLeft.text = "";
            fuelLeft.text = "";
            oldUnit = null;
        } else {
            endTurnButtonParent.SetActive(false);
            nextUnitButtonParent.SetActive(true);

            if (UIData.Instance.currentUnit != null) {
                healthSlider.maxValue = UIData.Instance.currentUnit.maxHealth;
                healthSlider.value = UIData.Instance.currentUnit.health;

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

                // If a different unit has been selected, disable the UnitUI Line Renderer, and set MoveButton to interactable 
                if (!moveButton.interactable && UIData.Instance.currentUnit != oldUnit) {
                    oldUnit = UIData.Instance.currentUnit;
                    moveButton.interactable = true;
                    unitUI.Hide();
                }

                // If the right mouse button is pressed while the line is showing, disable the UnitUI Line Renderer, and set Move Button to interactable
                if (Input.GetMouseButtonDown(1) && unitUI.showLine) {
                    unitUI.Hide();
                    moveButton.interactable = true;
                    unitIsMoving = false;
                }

                // If the left mouse button is pressed while the line is showing, disable the UnitUI Line Renderer, set Move Button to interactable, and move the selected Unit
                if (Input.GetMouseButtonDown(0) && unitUI.showLine) {
                    unitUI.Hide();
                    moveButton.interactable = true;
                    unitIsMoving = false;
                    UIData.Instance.Move();
                }
            } else {
                customButtonParent.SetActive(false);
                endTurnButtonParent.SetActive(false);
                nextUnitButtonParent.SetActive(true);
                movesLeft.text = "";
                fuelLeft.text = "";
                SetButtons(false);
                
                oldUnit = null;
                GameManager.Instance.grid.path = null;
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
            unitUI.Hide();
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.Sleeping) {
            sleepButtonParent.SetActive(false);
            wakeButtonParent.SetActive(true);
            moveButton.interactable = false;
            sleepButton.interactable = false;
            doneButton.interactable = false;
            wakeButton.interactable = true;
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.Complete) {
            SetButtons(false);
        } else {
            sleepButton.interactable = true;
            wakeButton.interactable = true;
            doneButton.interactable = true;
            sleepButtonParent.SetActive(true);
            wakeButtonParent.SetActive(false);
            if (unitIsMoving) {
                moveButton.interactable = false;
            } else {
                moveButton.interactable = true;
            }
        }
    }

    public void SetButtons(bool isActive) {
        moveButton.interactable = isActive;
        sleepButton.interactable = isActive;
        wakeButton.interactable = isActive;
        doneButton.interactable = isActive;
    }

    #region Delegate Functions

    public void NewDay() {
        newDayUI.SetActive(true);
        StartCoroutine(NewDayWait(newDayWaitTime));
    }

    IEnumerator NewDayWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        newDayUI.SetActive(false);
        GameManager.Instance.NewDay();
    }

    public void NextPlayer() {
        if (nextPlayerUIEnabled) {
            nextPlayerUI.SetActive(true);
            GameManager.Instance.Pause();
            nextPlayerText.text = $"Player {GameManager.Instance.currentPlayerIndex}'s Turn";
        }
        nextPlayerUIEnabled = true;
    }

    #endregion

    #region  Button Functions

    public void MoveButton() {
        if (UIData.Instance.currentUnit == null)  { return; }

        oldUnit = UIData.Instance.currentUnit;
        unitUI.Show();
        moveButton.interactable = false;
        unitIsMoving = true;
    }

    public void SleepButton() {
        if (UIData.Instance.currentUnit == null)  { return; }

        UIData.Instance.currentUnit.ToggleSleep();
    }

    public void WakeButton() {
        if (UIData.Instance.currentUnit == null)  { return; }

        UIData.Instance.currentUnit.ToggleSleep();
    }

    public void DoneButton() {
        if (UIData.Instance.currentUnit == null)  { return; }

        UIData.Instance.currentUnit.EndTurn();
    }

    public void CustomButton() {
        if (UIData.Instance.currentUnit == null)  { return; }
        
        ICustomButton currentUnitInterface = UIData.Instance.currentUnit as ICustomButton;
        if (currentUnitInterface != null) {
            currentUnitInterface.CustomButton();
        }
    }

    public void EndTurnButton() {
        GameManager.Instance.GetCurrentPlayer().TurnComplete();
    }

    public void NextUnitButton() {
        if (UIData.Instance.currentUnit == null)  { 
            Unit currentUnit = GameManager.Instance.GetCurrentPlayer().GetCurrentUnit();
            if (currentUnit != null) {
                UIData.Instance.currentUnit = currentUnit;
                UIData.Instance.currentCity = null;
            }
        }

        cameraController.Focus(GridUtilities.TileToWorldPos(UIData.Instance.currentUnit.pos), true);

        GameManager.Instance.GetCurrentPlayer().NextUnit(UIData.Instance.currentUnit, true);
    }

    public void NextPlayerButton() {
        nextPlayerUI.SetActive(false);
        GameManager.Instance.Resume();
    }

    #endregion
}