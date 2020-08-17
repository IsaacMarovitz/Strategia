using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour {

    public UIInfo UIInfo;
    public TMP_Text movesLeft;
    public TMP_Text dayCounter;

    void Update() {
        dayCounter.text = "Day " + UIInfo.day;
        if (UIInfo.unitSelected) {
            movesLeft.text = "Moves Left: " + UIInfo.movesLeft;
        } else {
            movesLeft.text = "";
        }
    }
}
