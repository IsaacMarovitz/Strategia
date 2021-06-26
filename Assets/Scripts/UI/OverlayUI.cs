using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OverlayUI : MonoBehaviour {

    public TMP_Text newDayUIText;
    public TMP_Text nextPlayerText;
    public Button nextPlayerButton;
    public GameObject newDayUI;
    public GameObject nextPlayerUI;
    public float newDayWaitTime = 1f;

    void Start() {
        nextPlayerButton.onClick.AddListener(NextPlayerButton);
        GameManager.Instance.newDayDelegate += NewDay;
        GameManager.Instance.nextPlayerDelegate += NextPlayer;
    }

    void Update() {
        newDayUIText.text = $"Day {GameManager.Instance.day + 1}";
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

    public void NextPlayer() {
        nextPlayerUI.SetActive(true);
        GameManager.Instance.Pause();
        nextPlayerText.text = $"Player {GameManager.Instance.currentPlayerIndex}'s Turn";
    }

    public void NextPlayerButton() {
        nextPlayerUI.SetActive(false);
        GameManager.Instance.Resume();
    }
}
