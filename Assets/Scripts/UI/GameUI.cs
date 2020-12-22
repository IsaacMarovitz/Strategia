using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour {

    public TMP_Text movesLeft;
    public TMP_Text dayCounter;
    public TMP_Text newDayUIText;
    public TMP_Text sleepButtonText;
    public TMP_Text nextPlayerText;
    public Button sleepButton;
    public Button laterButton;
    public Button doneButton;
    public Button nextPlayerButton;
    public Image unitImage;
    public GameObject newDayUI;
    public float newDayWaitTime;
    public GameObject nextPlayerUI;
    public bool nextPlayerUIEnabled = true;

    public void Start() {
        sleepButton.onClick.AddListener(Sleep);
        laterButton.onClick.AddListener(Later);
        doneButton.onClick.AddListener(Done);
        nextPlayerButton.onClick.AddListener(NextPlayerButton);
        GameManager.Instance.newDayDelegate += NewDay;
        GameManager.Instance.nextPlayerDelegate += NextPlayer;
    }

    void Update() {
        dayCounter.text = "Day " + GameManager.Instance.day;
        newDayUIText.text = "Day " + (GameManager.Instance.day+1);
        if (UIData.Instance.currentUnit != null) {
            movesLeft.text = "Moves Left: " + UIData.Instance.currentUnit.moves;
            sleepButton.interactable = true;
            laterButton.interactable = true;
            doneButton.interactable = true;
        } else {
            movesLeft.text = "";
            sleepButton.interactable = false;
            laterButton.interactable = false;
            doneButton.interactable = false;
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
}
