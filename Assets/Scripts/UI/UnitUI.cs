using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour {

    public Canvas canvas;
    public Button[] buttons = new Button[9];
    public float yOffset = 1f;

    public void Start() {
        canvas.enabled = false;
    }

    public void Update() {
        if (UIData.Instance.currentUnit != null) {
            canvas.enabled = true;
            transform.position = new Vector3(UIData.Instance.currentUnit.transform.position.x, yOffset, UIData.Instance.currentUnit.transform.position.z);

            for (int i = 0; i < buttons.Length; i++) {
                if (UIData.Instance.currentUnit.moveDirs[i] == MoveType.No) {
                    buttons[i].interactable = false;
                    buttons[i].targetGraphic.color = Color.white;
                } else if (UIData.Instance.currentUnit.moveDirs[i] == MoveType.Attack) {
                    buttons[i].interactable = true;
                    buttons[i].targetGraphic.color = Color.red;
                } else if (UIData.Instance.currentUnit.moveDirs[i] == MoveType.Move) {
                    buttons[i].interactable = true;
                    buttons[i].targetGraphic.color = Color.white;
                }
            }
        } else {
            canvas.enabled = false;
        }
    }


    public void Move(int dir) {
        UIData.Instance.Move(dir);
    }
}