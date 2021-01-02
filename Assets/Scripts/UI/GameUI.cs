using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour {

    public TMP_Text movesLeft;
    public TMP_Text fuelLeft;
    public TMP_Text dayCounter;
    public TMP_Text newDayUIText;
    public TMP_Text sleepButtonText;
    public TMP_Text customButtonText;
    public TMP_Text nextPlayerText;
    public Button moveButton;
    public Button sleepButton;
    public Button laterButton;
    public Button doneButton;
    public Button customButton;
    public Button endTurnButton;
    public Button nextPlayerButton;
    public Image unitImage;
    public GameObject customButtonParent;
    public GameObject newDayUI;
    public float newDayWaitTime;
    public GameObject nextPlayerUI;
    public bool nextPlayerUIEnabled = true;
    public UnitUI unitUI;

    private Unit oldUnit;

    public void Start() {
        moveButton.onClick.AddListener(Move);
        sleepButton.onClick.AddListener(Sleep);
        laterButton.onClick.AddListener(Later);
        doneButton.onClick.AddListener(Done);
        endTurnButton.onClick.AddListener(EndTurn);
        nextPlayerButton.onClick.AddListener(NextPlayerButton);
        GameManager.Instance.newDayDelegate += NewDay;
        GameManager.Instance.nextPlayerDelegate += NextPlayer;
        oldUnit = UIData.Instance.currentUnit;
    }

    void Update() {
        dayCounter.text = "Day " + GameManager.Instance.day;
        newDayUIText.text = "Day " + (GameManager.Instance.day + 1);
        customButtonParent.SetActive(false);
        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            movesLeft.text = "";
            fuelLeft.text = "";
            moveButton.interactable = false;
            sleepButton.interactable = false;
            laterButton.interactable = false;
            doneButton.interactable = false;
            endTurnButton.interactable = true;
            customButtonParent.SetActive(false);
            oldUnit = null;
        } else {
            endTurnButton.interactable = false;
            if (UIData.Instance.currentUnit != null) {
                movesLeft.text = "Moves Left: " + UIData.Instance.currentUnit.moves;
                fuelLeft.text = "";
                if (UIData.Instance.currentUnit.GetType() == typeof(Bomber)) {
                    fuelLeft.text = "Fuel: " + ((Bomber)UIData.Instance.currentUnit).fuel;
                    customButtonText.text = "Detonate";
                    customButton.onClick.RemoveAllListeners();
                    customButton.onClick.AddListener(Detonate);
                    customButtonParent.SetActive(true);
                } else if (UIData.Instance.currentUnit.GetType() == typeof(Fighter)) {
                    fuelLeft.text = "Fuel: " + ((Fighter)UIData.Instance.currentUnit).fuel;
                } else if (UIData.Instance.currentUnit.GetType() == typeof(Parachute)) {
                    fuelLeft.text = "Fuel: " + ((Parachute)UIData.Instance.currentUnit).fuel;
                    customButtonText.text = "Deploy";
                    customButton.onClick.RemoveAllListeners();
                    customButton.onClick.AddListener(Deploy);
                    customButtonParent.SetActive(true);
                } else {
                    fuelLeft.text = "";
                }
                sleepButton.interactable = true;
                laterButton.interactable = true;
                doneButton.interactable = true;
                if (!moveButton.interactable && UIData.Instance.currentUnit != oldUnit) {
                    oldUnit = UIData.Instance.currentUnit;
                    moveButton.interactable = true;
                    unitUI.showLine = false;
                }
                if (Input.GetMouseButtonDown(1) && unitUI.showLine) {
                    unitUI.showLine = false;
                    moveButton.interactable = true;
                } else if (Input.GetMouseButtonDown(0) && unitUI.showLine) {
                    unitUI.showLine = false;
                    moveButton.interactable = true;
                    UIData.Instance.Move();
                }
            } else {
                movesLeft.text = "";
                fuelLeft.text = "";
                moveButton.interactable = false;
                sleepButton.interactable = false;
                laterButton.interactable = false;
                doneButton.interactable = false;
                customButtonParent.SetActive(false);
                oldUnit = null;
            }
            if (!GameManager.Instance.dayCompleted) {
                if (UIData.Instance.currentUnit != null)
                    UpdateUI();
            } else {
                SetButtons(false);
            }
        }
    }

    public void NewDay() {
        newDayUI.SetActive(true);
        StartCoroutine(NewDayWait(newDayWaitTime));
    }

    IEnumerator NewDayWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        newDayUI.SetActive(false);
        GameManager.Instance.NewDay();
    }

    public void NextPlayerButton() {
        nextPlayerUI.SetActive(false);
        GameManager.Instance.Resume();
    }

    public void NextPlayer() {
        if (nextPlayerUIEnabled) {
            nextPlayerUI.SetActive(true);
            GameManager.Instance.Pause();
            nextPlayerText.text = $"Player {GameManager.Instance.currentPlayerIndex}'s Turn";
        }
    }

    public void UpdateUI() {
        if (UIData.Instance.currentUnit.turnStage == TurnStage.Waiting) {
            UIData.Instance.currentUnit.StartTurn();
            unitUI.showLine = false;
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.Sleeping) {
            SetButtons(false);
            sleepButton.interactable = true;
            sleepButtonText.text = "Wake";
        } else if (UIData.Instance.currentUnit.turnStage == TurnStage.Complete) {
            SetButtons(false);
        } else {
            SetButtons(true);
            sleepButtonText.text = "Sleep";
        }
        unitImage.color = GameManager.Instance.GetCurrentPlayer().playerColor;
    }

    void SetButtons(bool buttonBool) {
        sleepButton.interactable = buttonBool;
        laterButton.interactable = buttonBool;
        doneButton.interactable = buttonBool;
    }

    public void Move() {
        if (UIData.Instance.currentUnit != null) {
            unitUI.showLine = true;
            moveButton.interactable = false;
            //UIData.Instance.currentUnit.Move();
        }
    }

    public void Sleep() {
        if (UIData.Instance.currentUnit != null) {
            UIData.Instance.currentUnit.ToggleSleep();
        }
    }

    public void Later() {
        if (UIData.Instance.currentUnit != null) {
            UIData.Instance.currentUnit.Later();
        }
    }

    public void Done() {
        if (UIData.Instance.currentUnit != null) {
            UIData.Instance.currentUnit.EndTurn();
        }
    }

    public void Detonate() {
        if (UIData.Instance.currentUnit != null) {
            if (UIData.Instance.currentUnit.GetType() == typeof(Bomber)) {
                ((Bomber)UIData.Instance.currentUnit).Detonate();
            }
        }
    }

    public void Deploy() {
        if (UIData.Instance.currentUnit != null) {
            if (UIData.Instance.currentUnit.GetType() == typeof(Parachute)) {
                ((Parachute)UIData.Instance.currentUnit).DeployArmy();
            }
        }
    }

    public void EndTurn() {
        GameManager.Instance.GetCurrentPlayer().TurnComplete();
    }
}
