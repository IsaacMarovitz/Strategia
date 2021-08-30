using UnityEngine;
using System.Collections;

public class TileTooltip : TurnBehaviour {

    public float tooltipDelay = 2;
    private Tile mouseOverTile;

    public override void OnMouseOverTileSelected(Tile tile) {
        TooltipSystem.Hide();

        if (GameManager.Instance.GetCurrentPlayer().fogOfWarMatrix[tile.pos.x, tile.pos.y] != FogOfWarState.Hidden) {
            mouseOverTile = tile;
            StartCoroutine(Delay());
        }
    }

    public override void OnMouseOverTileDeselected() {
        mouseOverTile = null;
        TooltipSystem.Hide();
    }

    string TileTypeToString(TileType tileType) {
        switch (tileType) {
            case TileType.Sea:
                return "Sea";
            case TileType.Plains:
                return "Plains";
            case TileType.Swamp:
                return "Swamp";
            case TileType.Mountains:
                return "Mountains";
            case TileType.Trees:
                return "Trees";
            case TileType.City:
                return "City";
            case TileType.CostalCity:
                return "Costal City";
        }

        return "";
    }

    IEnumerator Delay() {
        yield return new WaitForSeconds(tooltipDelay);
        if (mouseOverTile != null) {
            TooltipSystem.Show($"{mouseOverTile.pos.x}, {mouseOverTile.pos.y}", $"{TileTypeToString(mouseOverTile.tileType)}");
        }
    }
}