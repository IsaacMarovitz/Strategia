using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

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
    public Action<UnitType, bool> updateToggleDelegate;

    private City oldCity;
    private bool hasUpdated = false;
    private Sequence panelCanvasGroupSequence;

    public void Start() {
        panel.SetActive(false);
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

            bool isCostalCity = GameManager.Instance.tileGrid.grid[city.pos.x, city.pos.y].tileType == TileType.CostalCity;
            updateToggleDelegate?.Invoke(city.unitType, isCostalCity);
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

    public void ChangeUnitType(bool value, UnitType unitType) {
        if (!value) { return; }
        if (UIData.currentCity == null) { return; }

        UIData.currentCity.UpdateUnitType(unitType);
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