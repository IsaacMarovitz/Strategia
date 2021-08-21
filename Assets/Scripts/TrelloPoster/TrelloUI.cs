using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Trello;

public class TrelloUI : MonoBehaviour {
	
	public Button reportButton;
    public FailureUI failureUI;
    public DragWindow successDragWindow;
    [SerializeField] private TrelloPoster trelloPoster = null;
    [SerializeField] private GameObject reportPanel = null;
    [SerializeField] private DragWindow dragWindow = null;
    [SerializeField] private TMP_InputField cardName = null;
    [SerializeField] private TMP_InputField cardDesc = null;
    [SerializeField] private Toggle includeScreenshot = null;

    private Texture2D screenshot;
	private GameManager gameManager;

	public void Start() {
		gameManager = GameObject.FindObjectOfType<GameManager>();
        if (trelloPoster == null) {
            Debug.LogWarning($"<b>{this.gameObject.name}:</b> No assigned Trello Poster SO!");
        }
	}

    public void StartPostCard() {
		StartCoroutine(PostCard());
    }

	IEnumerator PostCard() {
        if (trelloPoster != null) {
            string nameText = cardName.text;
            string descText = cardDesc.text;
            ResetUI();
            reportPanel.SetActive(false);
            yield return new WaitForEndOfFrame();
            TakeScreenshot();
            StartCoroutine(trelloPoster.PostCard(new TrelloCard(nameText, descText, trelloPoster.CardList, trelloPoster.CardLabel, includeScreenshot.isOn ? screenshot.EncodeToPNG() : null), Failure));
            Resume();
        } else {
            Debug.LogWarning($"<b>{this.gameObject.name}:</b> No assigned Trello Poster SO!");
        }
	}

    private List<Dropdown.OptionData> GetDropdownOptions(TrelloCardOption[] cardOptions) {
        List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < cardOptions.Length; i++) {
            dropdownOptions.Add(new Dropdown.OptionData(cardOptions[i].Name));
        }
        return dropdownOptions;
    }

	public void Pause() {
		gameManager.Pause();
        dragWindow.Open(() => reportButton.interactable = false);
	}

	public void Resume() {
		gameManager.Resume();
        dragWindow.Close(() => reportButton.interactable = true);
	}

    public void TakeScreenshot() {
        screenshot = ScreenCapture.CaptureScreenshotAsTexture();
    }

    public void ResetUI() {
        cardName.text = "";
        cardDesc.text = "";
    }

    public void Failure(string error) {
        if (!string.IsNullOrEmpty(error)) {
            failureUI.DisplayError(error);
        } else {
            successDragWindow.Open(() => {});
        }
    }
}
