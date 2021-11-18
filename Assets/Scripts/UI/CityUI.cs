using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class CityUI : TurnBehaviour {

    public GameObject panel;
    public CanvasGroup panelCanvasGroup;
    public float fadeDuration = 0.25f;
    public TMP_Text cityName;
    public TMP_Text turnsLeft;
    public TMP_InputField inputField;
    public float yOffset;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject unitButtonPrefab;
    public List<Toggle> costalCityToggles;
    public Toggle[] toggles;

    private City oldCity;
    private bool hasUpdated = false;
    private Sequence panelCanvasGroupSequence;

    public void Start() {
        panel.SetActive(false);
        for (int i = 0; i < toggles.Length; i++) {
            Toggle toggle = toggles[i];
            int tempInt = i;
            toggles[i].onValueChanged.AddListener(delegate { ChangeUnitType(toggle, tempInt); });
        }
    }

    public override void OnDestroy() {
        base.OnDestroy();
        if (oldCity != null) {
            oldCity.fastProdDelegate -= UpdateTurnsLeft;
        }
    }

    public void UpdateTurnsLeft() {
        turnsLeft.text = "Days Left: " + oldCity.turnsLeft;
    }

    public override void OnCitySelected(City city) {
        city.fastProdDelegate += UpdateTurnsLeft;
        city.showCityNameUI = false;
        panel.SetActive(true);
        panelCanvasGroup.interactable = false;

        panelCanvasGroupSequence.Kill();
        panelCanvasGroupSequence = DOTween.Sequence();
        panelCanvasGroupSequence
            .Append(panelCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutCubic))
            .OnComplete(() => {
                panelCanvasGroup.interactable = true;
            })
            .Play();

        transform.position = GridUtilities.TileToWorldPos(city.pos, yOffset);
        cityName.text = city.cityName;
        turnsLeft.text = "Days Left: " + city.turnsLeft;
        if (oldCity != city) {
            hasUpdated = false;
            if (oldCity != null) {
                oldCity.showCityNameUI = true;
            }
            oldCity = city;
        }
        if (!hasUpdated) {
            hasUpdated = true;
            UpdateUnitButtons();
            for (int i = 0; i < toggles.Length; i++) {
                if (i == (int)city.unitType) {
                    toggles[i].SetIsOnWithoutNotify(true);
                } else {
                    toggles[i].SetIsOnWithoutNotify(false);
                }
            }
            if (GameManager.Instance.tileGrid.grid[city.pos.x, city.pos.y].tileType == TileType.CostalCity) {
                foreach (var toggle in costalCityToggles) {
                    toggle.interactable = true;
                }
            } else {
                foreach (var toggle in costalCityToggles) {
                    toggle.interactable = false;
                }
            }
        }
    }

    public override void OnCityDeselected() {
        panelCanvasGroupSequence.Kill();
        panelCanvasGroupSequence = DOTween.Sequence();
        panelCanvasGroupSequence
            .Append(panelCanvasGroup.DOFade(0, fadeDuration).SetEase(Ease.InOutCubic))
            .OnComplete(() => {
                panel.SetActive(false);
            })
            .Play();
        
        hasUpdated = false;
        if (oldCity != null) {
            oldCity.fastProdDelegate -= UpdateTurnsLeft;
            oldCity.showCityNameUI = true;
        }
    }

    public void ChangeUnitType(Toggle toggle, int tempInt) {
        if (!toggle.isOn) { return; }
        if (UIData.currentCity == null) { return; }

        UIData.currentCity.UpdateUnitType((UnitType)tempInt);
        turnsLeft.text = "Days Left: " + UIData.currentCity.turnsLeft;
    }

    public void UpdateUnitButtons() {
        if (UIData.currentCity == null) { return; }

        for (int i = horizontalLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
        }
        
        foreach (var unit in UIData.currentCity.unitsInCity) {
            GameObject newButton = GameObject.Instantiate(unitButtonPrefab, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(horizontalLayoutGroup.transform, false);
            UnitButtonUI unitButton = newButton.GetComponent<UnitButtonUI>();
            unitButton.unit = unit;
            if (unit.unitIcon != null) {
                unitButton.image.sprite = unit.unitIcon;
            }
        }
    }

    public void ShowInputField() {
        if (UIData.currentCity == null) { return; }

        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        inputField.text = UIData.currentCity.cityName;
        GameManager.Instance.Pause();
    }

    public void FinishChangingName() {
        if (UIData.currentCity == null) { return; }

        UIData.currentCity.cityName = inputField.text.Trim();
        cityName.text = UIData.currentCity.cityName;
        inputField.gameObject.SetActive(false);
        GameManager.Instance.Resume();
    }
}