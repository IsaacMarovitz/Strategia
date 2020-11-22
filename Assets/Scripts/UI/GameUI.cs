using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour {

    public TMP_Text movesLeft;
    public TMP_Text dayCounter;
    public TMP_Text newDayUIText;
    public TMP_Text sleepButtonText;
    public Button sleepButton;
    public Button laterButton;
    public Button doneButton;
    public GameObject newDayUI;
    public float newDayWaitTime;

    public void Start() {
        sleepButton.onClick.AddListener(Sleep);
        laterButton.onClick.AddListener(Later);
        doneButton.onClick.AddListener(Done);
        GameManager.Instance.newDayDelegate += NewDay;
    }

    void Update() {
        dayCounter.text = "Day " + GameManager.Instance.day;
        newDayUIText.text = "Day " + (GameManager.Instance.day+1);
        if (UIData.Instance.currentUnit != null) {
            movesLeft.text = "Moves Left: " + UIData.Instance.currentUnit.movesLeft;
        } else {
            movesLeft.text = "";
        }
        if (!GameManager.Instance.dayCompleted) {
            if (UIData.Instance.currentUnit != null) 
                UpdateUI();
        } else {
            SetButtons(false);
        }
    }

    public void NewDay() {
        newDayUI.SetActive(true);
        StartCoroutine(Wait(newDayWaitTime));
    }

    IEnumerator Wait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        newDayUI.SetActive(false);
        GameManager.Instance.NewDay();
    }

    public void UpdateUI() {
        if (!UIData.Instance.currentUnit.turnStarted) {
            UIData.Instance.currentUnit.StartTurn();
        } else if (UIData.Instance.currentUnit.isSleeping) {
            SetButtons(false);
            sleepButton.interactable = true;
            sleepButtonText.text = "Wake";
        } else if (UIData.Instance.currentUnit.turnComplete) {
            SetButtons(false);
        } else {
            SetButtons(true);
            sleepButtonText.text = "Sleep";
        }
    }

    void SetButtons(bool buttonBool) {
        sleepButton.interactable = buttonBool;
        laterButton.interactable = buttonBool;
        doneButton.interactable = buttonBool;
    }

    public void Sleep() {
        UIData.Instance.currentUnit.ToggleSleep();
    }

    public void Later() {
        UIData.Instance.currentUnit.Later();
    }

    public void Done() {
        UIData.Instance.currentUnit.Done();
    }
}
