using UnityEngine;
using UnityEngine.UI;

public class ShowMinimap : TurnBehaviour {

    public RawImage image;

    public override void OnPlayerTurnStart(Player player) {
        image.texture = player.minimapTexture;
    }
}
