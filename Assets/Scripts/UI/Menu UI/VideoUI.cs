using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;

public class VideoUI : MonoBehaviour {

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown aaDropdown;
    public Toggle vsyncToggle;
    public Toggle fullscreenToggle;
    public Button backButton;
    public GameObject pauseUI;

    private Resolution[] resolutions;
    private int aaSamples;
    private UniversalRenderPipelineAsset urpAsset;
    private int vsync;

    void Start() {
        resolutions = Screen.resolutions.Where(resolution => resolution.refreshRate == Screen.currentResolution.refreshRate || resolution.refreshRate == Screen.currentResolution.refreshRate-1).ToArray();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(Resolution);

        qualityDropdown.ClearOptions();
        List<string> quality = new List<string>();
        for (int i = 0; i < QualitySettings.names.Length; i++) {
            quality.Add(QualitySettings.names[i]);
        }
        qualityDropdown.AddOptions(quality);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(Quality);

        urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
        aaSamples = urpAsset.msaaSampleCount;
        if (aaSamples == 0) {
            aaDropdown.value = 0;
        } else if (aaSamples == 2) {
            aaDropdown.value = 1;
        } else if (aaSamples == 4) {
            aaDropdown.value = 2;
        } else if (aaSamples == 8) {
            aaDropdown.value = 3;
        } else {
            aaDropdown.value = 0;
        }
        aaDropdown.onValueChanged.AddListener(AA);
        if (QualitySettings.vSyncCount > 0) {
            vsyncToggle.isOn = true;
            vsync = QualitySettings.vSyncCount;
        } else {
            vsyncToggle.isOn = false;
            vsync = QualitySettings.vSyncCount;
        }
        vsyncToggle.onValueChanged.AddListener(VSync);
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(Fullscreen);
        backButton.onClick.AddListener(Back);
    }

    public void Resolution(int index) {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
    }

    public void Quality(int index) {
        QualitySettings.SetQualityLevel(index);
        urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
        urpAsset.msaaSampleCount = aaSamples;
        QualitySettings.vSyncCount = vsync;
    }

    public void AA(int index) {
        if (index == 0) {
            aaSamples = 0;
        } else if (index == 1) {
            aaSamples = 2;
        } else if (index == 2) {
            aaSamples = 4;
        } else if (index == 3) {
            aaSamples = 8;
        } else {
            aaSamples = 0;
        }
        urpAsset.msaaSampleCount = aaSamples;
    }

    public void VSync(bool isVsync) {
        if (isVsync) {
            QualitySettings.vSyncCount = 1;
            vsync = 1;
        } else {
            QualitySettings.vSyncCount = 0;
            vsync = 0;
        }
    }

    public void Fullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void Back() {
        pauseUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
