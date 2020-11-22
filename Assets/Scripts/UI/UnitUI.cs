using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour {

    public Canvas canvas;
    public Button UL;
    public Button U;
    public Button UR;
    public Button L;
    public Button R;
    public Button DL;
    public Button D;
    public Button DR;

    public void Start() {
        canvas.enabled = false;
    }

    public void Update() {
        if (UIData.Instance.currentUnit != null) {
            canvas.enabled = true;
            transform.position = new Vector3(UIData.Instance.currentUnit.transform.position.x, 0.5f, UIData.Instance.currentUnit.transform.position.z);

            UL.interactable = true;
            U.interactable = true;
            UR.interactable = true;
            L.interactable = true;
            R.interactable = true;
            DL.interactable = true;
            D.interactable = true;
            DR.interactable = true;
            if (!UIData.Instance.currentUnit.moveDirs[0]) {
                UL.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[1]) {
                U.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[2]) {
                UR.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[3]) {
                L.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[4]) {
                R.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[5]) {
                DL.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[6]) {
                D.interactable = false;
            }
            if (!UIData.Instance.currentUnit.moveDirs[7]) {
                DR.interactable = false;
            }

        } else {
            canvas.enabled = false;
        }
    }

    
    public void Move(int dir) {
        UIData.Instance.Move(dir);
    }
}