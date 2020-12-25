using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour {

    public Canvas canvas;
    public Button[] buttons = new Button[9];
    public float yOffset = 1f;

    public void Update() {
        if (UIData.Instance.currentUnit != null) {
            transform.position = new Vector3(UIData.Instance.currentUnit.transform.position.x, yOffset, UIData.Instance.currentUnit.transform.position.z);

            for (int i = 0; i < buttons.Length; i++) {
                if (UIData.Instance.currentUnit.moves > 0 && UIData.Instance.currentUnit.turnStage != TurnStage.Sleeping) {
                    if (UIData.Instance.currentUnit.moveDirs[i] == TileMoveStatus.Blocked) {
                        buttons[i].interactable = false;
                        buttons[i].targetGraphic.color = Color.white;
                    } else if (UIData.Instance.currentUnit.moveDirs[i] == TileMoveStatus.Attack) {
                        buttons[i].interactable = true;
                        buttons[i].targetGraphic.color = Color.red;
                    } else if (UIData.Instance.currentUnit.moveDirs[i] == TileMoveStatus.Move) {
                        buttons[i].interactable = true;
                        buttons[i].targetGraphic.color = Color.white;
                    } else if (UIData.Instance.currentUnit.moveDirs[i] == TileMoveStatus.Transport) {
                        buttons[i].interactable = true;
                        buttons[i].targetGraphic.color = Color.white;
                    }
                } else {
                    buttons[i].interactable = false;
                }
            }
            canvas.enabled = true;
        } else {
            canvas.enabled = false;
        }
    }


    public void Move(int dir) {
        UIData.Instance.Move(dir);
    }
}