using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {

    [Header("Text")]
    public TMP_Text movesLeft;
    public TMP_Text fuelLeft;
    public TMP_Text dayCounter;
    public TMP_Text customButtonText;

    [Header("Misc")]
    public Image unitImage;
    public Slider healthSlider;
    public CameraController cameraController;

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

    public void Update() {
        dayCounter.text = $"Day {GameManager.Instance.day}";
        unitImage.color = GameManager.Instance.GetCurrentPlayer().playerColor;

        if (GameManager.Instance.GetCurrentPlayer().turnCompleted) {
            movesLeft.text = "";
            fuelLeft.text = "";
            oldUnit = null;

            if (currentUnit != null) {
                if (currentUnit.unitIcon != null) {
                    unitImage.sprite = currentUnit.unitIcon;
                }
            }
        } else {
            if (currentUnit != null) {
                UnitMoveUI unitMoveUI = currentUnit.unitMoveUI;

                healthSlider.maxValue = currentUnit.maxHealth;
                healthSlider.value = currentUnit.health;
                if (currentUnit.unitIcon != null) {
                    unitImage.sprite = currentUnit.unitIcon;
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
                movesLeft.text = "";
                fuelLeft.text = "";

                oldUnit = null;
            }

            if (!GameManager.Instance.dayCompleted) {
                if (currentUnit != null)
                    UpdateUI();
            }
        }
    }

    public void UpdateUI() {
        if (currentUnit.turnStage == TurnStage.Waiting) {
            currentUnit.StartTurn();
        }
    }
}