using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Trello;

public class TrelloUI : MonoBehaviour {
	
	public Button reportButton;
    [SerializeField]
    private TrelloPoster trelloPoster;
    [SerializeField]
    private GameObject reportPanel;
    [SerializeField]
    private TMP_InputField cardName;
    [SerializeField]
    private TMP_InputField cardDesc;
    [SerializeField]
    private Toggle includeScreenshot;

    private Texture2D screenshot;
    private bool noLabels = false;
	private GameManager gameManager;

	public void Start() {
		gameManager = GameObject.FindObjectOfType<GameManager>();
	}

    public void StartPostCard() {
		StartCoroutine(PostCard());
    }

	IEnumerator PostCard() {
		string nameText = cardName.text;
		string descText = cardDesc.text;
		reportPanel.SetActive(false);
		yield return new WaitForEndOfFrame();
		TakeScreenshot();
		StartCoroutine(trelloPoster.PostCard(new TrelloCard(nameText, descText, trelloPoster.CardList, trelloPoster.CardLabel, includeScreenshot.isOn ? screenshot.EncodeToPNG() : null)));
		Resume();
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
		reportPanel.SetActive(true);
		reportButton.interactable = false;		
	}

	public void Resume() {
		gameManager.Resume();
		reportPanel.SetActive(false);
		reportButton.interactable = true;
	}

    public void TakeScreenshot() {
        screenshot = ScreenCapture.CaptureScreenshotAsTexture();
    }

    public void ResetUI() {
        cardName.text = "";
        cardDesc.text = "";
    }
}
