using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Trello;

public class ErrorUI : MonoBehaviour {

    public Button okButton;
    public Button reportButton;
    public DragWindow errorDragWindow;
    public FailureUI failureUI;
    public DragWindow successDragWindow;
    public TMP_Text errorText;
    public TrelloPoster trelloPoster;

    private string logString;
    private string errorString;

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        if (type != LogType.Log) {
            errorString = logString + "\n" + stackTrace;
            errorText.text = errorString;
            this.logString = logString;

            errorDragWindow.Open(() => {});
        }
    }

    void Start() {
        okButton.onClick.AddListener(Ok);
        reportButton.onClick.AddListener(Report);
        if (trelloPoster == null) {
            Debug.LogWarning($"<b>{this.gameObject.name}:</b> No assigned Trello Poster SO!");
        }
    }   

    void Ok() {
        errorDragWindow.Close(() => {});
    }

    void Report() {
        if (trelloPoster == null) {
            Debug.LogWarning($"<b>{this.gameObject.name}:</b> No assigned Trello Poster SO!");
            return;
        }
        StartCoroutine(trelloPoster.PostCard(new TrelloCard(logString, errorString, trelloPoster.CardList, trelloPoster.CardLabel, null), Failure));
        Ok();
    }

    public void Failure(string error) {
        if (!string.IsNullOrEmpty(error)) {
            failureUI.DisplayError(error);
        } else {
            successDragWindow.Open(() => {});
        }
    } 
}
