using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour {

    public Button resumeButton;
    public Button videoButton;
    public Button audioButton;
    public Button quitButton;

    public GameObject videoUI;
    public GameObject audioUI;
    
    public MenuUI menuUI;

    void Start() {
        resumeButton.onClick.AddListener(Resume);
        videoButton.onClick.AddListener(Video);
        audioButton.onClick.AddListener(Audio);
        quitButton.onClick.AddListener(Quit);
    }

    public void Resume () {
        menuUI.Resume();
    }

    public void Video () {
        videoUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void Audio() {
        audioUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }
}
