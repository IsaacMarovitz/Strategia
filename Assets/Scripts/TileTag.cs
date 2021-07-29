using UnityEngine;
using System.Collections.Generic;

public class TileTag : TurnBehaviour {
    public Vector2Int pos;
    public List<GameObject> gameObjects;

    public bool isEnabled = true;

    public override void OnFogOfWarUpdate(Player player) {
        if (player.fogOfWarMatrix[pos.x, pos.y] != FogOfWarState.Hidden) {
            if (!isEnabled) {
                isEnabled = true;
                foreach (var gameObject in gameObjects) {
                    gameObject.SetActive(true);
                }
            }
        } else {
            if (isEnabled) {
                isEnabled = false;
                foreach (var gameObject in gameObjects) {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}