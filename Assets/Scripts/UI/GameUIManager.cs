using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour {

    public UIInfo UIInfo;
    public TMP_Text movesLeft;
    public TMP_Text dayCounter;
    public TMP_Text sleepButtonText;
    public Button sleepButton;
    public Button laterButton;
    public Button doneButton;
    public GameManager gameManager;

    public void Start() {
        sleepButton.onClick.AddListener(Sleep);
        laterButton.onClick.AddListener(Later);
        doneButton.onClick.AddListener(Done);
    }

    void Update() {
        dayCounter.text = "Day " + UIInfo.day;
        if (UIInfo.unit != null) {
            movesLeft.text = "Moves Left: " + UIInfo.unit.movesLeft;
        } else {
            movesLeft.text = "";
        }
        if (!gameManager.dayCompleted) {
            if (UIInfo.unit != null) 
                UpdateUI();
        } else {
            SetButtons(false);
        }
    }

    public void UpdateUI() {
        if (!UIInfo.unit.turnStarted) {
            UIInfo.unit.StartTurn();
        } else if (UIInfo.unit.isSleeping) {
            SetButtons(false);
            sleepButton.interactable = true;
            sleepButtonText.text = "Wake";
        } else if (UIInfo.unit.turnComplete) {
            SetButtons(false);
        } else {
            SetButtons(true);
            sleepButtonText.text = "Sleep";
        }
    }

    void SetButtons(bool buttonBool) {
        sleepButton.interactable = buttonBool;
        laterButton.interactable = buttonBool;
        doneButton.interactable= buttonBool;
    }

    public void Sleep() {
        UIInfo.unit.ToggleSleep();
    }

    public void Later() {
        UIInfo.unit.Later();
    }

    public void Done() {
        UIInfo.unit.Done();
    }
}
