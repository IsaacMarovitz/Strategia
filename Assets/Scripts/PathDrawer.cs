using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour {

    public LineRenderer lineRenderer;
    public List<TileType> tileTypes;

    private Unit unit;
    private Tile city;
    private List<Tile> oldPositions;
    private Unit oldUnit;
    private Tile oldMouseOverTile;

    void Start() {
        unit = UIData.Instance.currentUnit;
        city = GameManager.Instance.grid.grid[unit.oldCity.pos.x, unit.oldCity.pos.y];
    }

    void Update() {
        if (UIData.Instance.currentUnit != null && UIData.Instance.mouseOverTile != null) {
            if (UIData.Instance.currentUnit != oldUnit || UIData.Instance.mouseOverTile != oldMouseOverTile) {
                GridUtilities.FindPath(GameManager.Instance.grid.grid[UIData.Instance.currentUnit.pos.x, UIData.Instance.currentUnit.pos.y], UIData.Instance.mouseOverTile, tileTypes);
            }
        }
        if (GameManager.Instance.grid.path != null && oldPositions != GameManager.Instance.grid.path) {
            oldPositions = GameManager.Instance.grid.path;
            lineRenderer.positionCount = GameManager.Instance.grid.path.Count;
            lineRenderer.SetPositions(TilesToWorldPositions(GameManager.Instance.grid.path));
        }
    }

    Vector3[] TilesToWorldPositions(List<Tile> tiles) {
        Vector3[] positions = new Vector3[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) {
            positions[i] = new Vector3(GameManager.Instance.grid.tileWidth * tiles[i].index.x, 0, GameManager.Instance.grid.tileHeight * tiles[i].index.y);
        }
        return positions;
    }
}
