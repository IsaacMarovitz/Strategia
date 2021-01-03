using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUI : MonoBehaviour {

    public LineRenderer lineRenderer;
    public TMP_Text numberOfMoves;
    public Canvas canvas;
    public bool showLine = false;

    private List<Tile> oldPositions;
    private Unit oldUnit;
    private Tile oldMouseOverTile;

    void Update() {
        if (UIData.Instance.currentUnit != null && UIData.Instance.mouseOverTile != null && showLine) {
            if (UIData.Instance.currentUnit != oldUnit || UIData.Instance.mouseOverTile != oldMouseOverTile) {
                lineRenderer.enabled = true;
                canvas.enabled = true;
                GridUtilities.FindPath(GameManager.Instance.grid.grid[UIData.Instance.currentUnit.pos.x, UIData.Instance.currentUnit.pos.y], UIData.Instance.mouseOverTile);
            }
        } else {
            lineRenderer.enabled = false;
            canvas.enabled = false;
        }
        if (GameManager.Instance.grid.path != null && oldPositions != GameManager.Instance.grid.path) {
            if (GameManager.Instance.grid.path.Count > 0) {
                oldPositions = GameManager.Instance.grid.path;
                lineRenderer.positionCount = GameManager.Instance.grid.path.Count;
                lineRenderer.SetPositions(TilesToWorldPositions(GameManager.Instance.grid.path));
                numberOfMoves.text = (GameManager.Instance.grid.path.Count - 1).ToString();
                int midIndex = Mathf.RoundToInt((GameManager.Instance.grid.path.Count - 1) / 2);
                canvas.transform.position = new Vector3(GameManager.Instance.grid.path[midIndex].gameObject.transform.position.x, 2, GameManager.Instance.grid.path[midIndex].gameObject.transform.position.z);
            }
        }
    }

    Vector3[] TilesToWorldPositions(List<Tile> tiles) {
        Vector3[] positions = new Vector3[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) {
            positions[i] = new Vector3(GameManager.Instance.grid.tileWidth * tiles[i].pos.x, 0, GameManager.Instance.grid.tileHeight * tiles[i].pos.y);
        }
        return positions;
    }
}
