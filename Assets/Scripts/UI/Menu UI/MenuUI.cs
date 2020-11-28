using UnityEngine;

public class MenuUI : MonoBehaviour {

    public GameObject menus;
    public GameObject pauseUI;
    public GameObject videoUI;
    public GameObject audioUI;

    private bool uiEnabled = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (uiEnabled) {
                Resume();
            } else {
                uiEnabled = true;
                GameManager.Instance.Pause();
                menus.SetActive(true);
                pauseUI.SetActive(true);
                videoUI.SetActive(false);
                audioUI.SetActive(false);
            }
        }
    }

    public void Resume() {
        uiEnabled = false;
        GameManager.Instance.Resume();
        menus.SetActive(false);
        pauseUI.SetActive(true);
        videoUI.SetActive(false);
        audioUI.SetActive(false);
    }
}
