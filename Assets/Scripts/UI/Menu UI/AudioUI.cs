using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioUI : MonoBehaviour {

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button backButton;

    public AudioMixer audioMixer;

    public GameObject pauseUI;

    void Start() {
        backButton.onClick.AddListener(Back);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume) {
        audioMixer.SetFloat("MasterVolume", (Mathf.Log10(volume) * 20));
    }

    public void SetMusicVolume(float volume) {
        audioMixer.SetFloat("MusicVolume", (Mathf.Log10(volume) * 20));
    }

    public void SetSFXVolume(float volume) {
        audioMixer.SetFloat("SFXVolume", (Mathf.Log10(volume) * 20));
    }

    public void Back() {
        pauseUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
