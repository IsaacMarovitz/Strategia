using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class OverlayUI : MonoBehaviour {

    public TMP_Text newDayUIText;
    public TMP_Text nextPlayerText;
    public Button nextPlayerButton;
    public GameObject newDayUI;
    public GameObject nextPlayerUI;
    public CanvasGroup nextPlayerUICanvasGroup;
    public float newDayWaitTime = 1f;
    public float transitionTime = 0.1f;

    void Start() {
        nextPlayerButton.onClick.AddListener(NextPlayerButton);
        GameManager.Instance.dayEndedDelegate += NewDay;
        GameManager.Instance.nextPlayerDelegate += NextPlayer;
        newDayUI.transform.localScale = new Vector3(1, 0, 1);
    }

    public void NewDay() {
        newDayUIText.text = $"Day {GameManager.Instance.day + 1}";
        newDayUI.SetActive(true);
        newDayUI.transform.DOScaleY(1f, transitionTime).OnComplete(() => {
            StartCoroutine(NewDayWait(newDayWaitTime));
        });
    }

    IEnumerator NewDayWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        newDayUI.transform.DOScaleY(0f, transitionTime).OnComplete(() => {
            newDayUI.SetActive(false);
            GameManager.Instance.NewDay();
        });
    }

    public void NextPlayer() {
        nextPlayerUI.SetActive(true);
        GameManager.Instance.Pause();
        nextPlayerText.text = $"Player {GameManager.Instance.currentPlayerIndex}'s Turn";
    }

    public void NextPlayerButton() {
        nextPlayerUICanvasGroup.interactable = false;
        nextPlayerUICanvasGroup.DOFade(0f, transitionTime).OnComplete(() => {
            nextPlayerUI.SetActive(false);
            nextPlayerUICanvasGroup.alpha = 1f;
            nextPlayerUICanvasGroup.interactable = true;
        });
        GameManager.Instance.Resume();
    }
}
