using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameUI : TurnBehaviour {

    [Header("Text")]
    public TMP_Text movesLeft;
    public TMP_Text fuelLeft;
    public TMP_Text dayCounter;
    public TMP_Text customButtonText;

    [Header("Misc")]
    public Image unitImage;
    public Slider healthSlider;
    public CameraController cameraController;
    public CanvasGroup iconUIGroup;
    public float fadeDuration = 0.25f;
    [Range(0, 1)]
    public float playerColorSaturation = 0.5f;
    [Range(0, 1)]
    public float playerColorBrightness = 0.5f;

    private Sequence iconUIGroupSequence;
    public Unit currentUnit {
        get {
            return UIData.currentUnit;
        }
        set {
            UIData.SetUnit(value);
        }
    }

    private bool startUnitEnabled = false;

    public override void OnPlayerStateChanged(Player player) {
        UpdateUI();
    }

    public override void OnPlayerTurnEnd(Player player) {
        startUnitEnabled = false;
    }

    public override void OnPlayerTurnStart(Player player) {
        startUnitEnabled = true;
    }

    public override void OnUnitAction() {
        UpdateUI();
    }

    public override void OnUnitTurnStart(Unit unit) {
        UpdateUI();
    }

    public override void OnUnitSelected(Unit unit) {
        UpdateUI();
    }

    public override void OnUnitDeselected() {
        UpdateUI();
    }

    public void UpdateUI() {
        dayCounter.text = $"Day {GameManager.Instance.day}";

        if (currentUnit != null) {
            UnitMoveUI unitMoveUI = currentUnit.unitMoveUI;

            iconUIGroup.gameObject.SetActive(true);
            iconUIGroupSequence.Kill();
            iconUIGroupSequence = DOTween.Sequence();
            iconUIGroupSequence
                .Append(iconUIGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutCubic));
            iconUIGroupSequence.Play();

            healthSlider.maxValue = currentUnit.maxHealth;
            healthSlider.value = currentUnit.health;
            if (currentUnit.unitIcon != null) {
                Color color = GameManager.Instance.GetCurrentPlayer().playerColor;
                Vector3 hsv = new Vector3();
                Color.RGBToHSV(color, out hsv.x, out hsv.y, out hsv.z);
                hsv.y = playerColorSaturation;
                hsv.z = playerColorBrightness;

                unitImage.color = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
                unitImage.sprite = currentUnit.unitIcon;
            } else {
                unitImage.color = Color.clear;
                unitImage.sprite = null;
            }

            movesLeft.text = $"Moves Left: {currentUnit.moves}";

            // If the unit inherits the IFuel interface, set FuelLeft tect to the current fuel level
            IFuel fuelInterface = currentUnit as IFuel;
            if (fuelInterface != null) {
                fuelLeft.text = $"Fuel: {fuelInterface.fuel}";
            } else {
                fuelLeft.text = "";
            }
        } else {
            iconUIGroupSequence.Kill();
            iconUIGroupSequence = DOTween.Sequence();
            iconUIGroupSequence
                .Append(iconUIGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutCubic))
                .OnComplete(() => {
                    iconUIGroup.gameObject.SetActive(false);
                });
            iconUIGroupSequence.Play();
        }

        if (!GameManager.Instance.dayCompleted && startUnitEnabled) {
            if (currentUnit != null)
                if (currentUnit.unitTurnStage == UnitTurnStage.Waiting) {
                    currentUnit.StartTurn();
                }
        }
    }
}