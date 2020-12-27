using UnityEngine;
using UnityEngine.UI;

public class ShowMinimap : MonoBehaviour {

    public RawImage image;

    void Update() {
        image.texture = GameManager.Instance.GetCurrentPlayer().minimapTexture;
    }
}
