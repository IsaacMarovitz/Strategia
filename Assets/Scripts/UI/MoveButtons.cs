using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MoveButtons : MonoBehaviour {

    public Canvas canvas;
    public Button UL;
    public Button U;
    public Button UR;
    public Button L;
    public Button R;
    public Button DL;
    public Button D;
    public Button DR;
    public UIInfo UIInfo;

    public void Start() {
        canvas.enabled = false;
    }

    public void Update() {
        if (UIInfo.unitSelected) {

            canvas.enabled = true;
            transform.position = new Vector3(UIInfo.unitWorldPos.x, 0.5f, UIInfo.unitWorldPos.z);

            UL.interactable = true;
            U.interactable = true;
            UR.interactable = true;
            L.interactable = true;
            R.interactable = true;
            DL.interactable = true;
            D.interactable = true;
            DR.interactable = true;
            if (!UIInfo.moveDirs[0]) {
                UL.interactable = false;
            }
            if (!UIInfo.moveDirs[1]) {
                U.interactable = false;
            }
            if (!UIInfo.moveDirs[2]) {
                UR.interactable = false;
            }
            if (!UIInfo.moveDirs[3]) {
                L.interactable = false;
            }
            if (!UIInfo.moveDirs[4]) {
                R.interactable = false;
            }
            if (!UIInfo.moveDirs[5]) {
                DL.interactable = false;
            }
            if (!UIInfo.moveDirs[6]) {
                D.interactable = false;
            }
            if (!UIInfo.moveDirs[7]) {
                DR.interactable = false;
            }

        } else {
            canvas.enabled = false;
        }
    }

    
    public void Move(int dir) {
        UIInfo.dir = dir;
        UIInfo.newMove = true;
    }
}