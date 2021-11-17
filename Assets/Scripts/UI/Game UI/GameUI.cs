using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public GameObject iconUIContainer;
    [Range(0, 1)]
    public float playerColorSaturation = 0.5f;
    [Range(0, 1)]
    public float playerColorBrightness = 0.5f;

    public Unit oldUnit;
    public Unit currentUnit {
        get {
            return UIData.currentUnit;
        }
        set {
            UIData.SetUnit(value);
        }
    }

    public void Start() {
        oldUnit = currentUnit;
    }

    public override void OnPlayerStateChanged(Player player) {
        UpdateUI();
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

            iconUIContainer.SetActive(true);
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
            iconUIContainer.SetActive(false);
            
            oldUnit = null;
        }

        if (!GameManager.Instance.dayCompleted) {
            if (currentUnit != null)
                if (currentUnit.unitTurnStage == UnitTurnStage.Waiting) {
                    // FIX
                    //currentUnit.StartTurn();
                }
        }
    }
}