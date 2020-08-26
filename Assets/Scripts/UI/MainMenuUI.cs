using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;
    [Space(10)]
    public GameObject newGameMenu;
    [Space(10)]
    public int levelSceneIndex;

    public void Start() {
        newGameButton.onClick.AddListener(NewGame);
        quitButton.onClick.AddListener(Quit);
    }

    public void NewGame() {
        /*newGameMenu.SetActive(true);
        this.transform.parent.gameObject.SetActive(false);*/
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSceneIndex);

        while(!asyncLoad.isDone) {
            yield return null;
        }
    }

    public void Quit() {
        Application.Quit();
    }
}
