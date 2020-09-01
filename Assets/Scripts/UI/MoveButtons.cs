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
        if (UIInfo.unit != null) {
            canvas.enabled = true;
            transform.position = new Vector3(UIInfo.unit.transform.position.x, 0.5f, UIInfo.unit.transform.position.z);

            UL.interactable = true;
            U.interactable = true;
            UR.interactable = true;
            L.interactable = true;
            R.interactable = true;
            DL.interactable = true;
            D.interactable = true;
            DR.interactable = true;
            if (!UIInfo.unit.moveDirs[0]) {
                UL.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[1]) {
                U.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[2]) {
                UR.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[3]) {
                L.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[4]) {
                R.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[5]) {
                DL.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[6]) {
                D.interactable = false;
            }
            if (!UIInfo.unit.moveDirs[7]) {
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