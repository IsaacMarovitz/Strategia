using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CancelMoveUI : MonoBehaviour {

    public LineRenderer lineRenderer;
    public TMP_Text numberOfMoves;
    public Canvas canvas;
    public List<Tile> path;

    private List<Tile> oldPositions;

    void Update() {
        if (UIData.Instance.currentUnit != null) {
            if (UIData.Instance.currentUnit.turnStage == TurnStage.PathSet && UIData.Instance.currentUnit.path != null) {
                lineRenderer.enabled = true;
                canvas.enabled = true;
                canvas.transform.position = GridUtilities.TileToWorldPos(UIData.Instance.currentUnit.pos, 2);
                path = UIData.Instance.currentUnit.path;
                path.Insert(0, GameManager.Instance.grid.grid[UIData.Instance.currentUnit.pos.x, UIData.Instance.currentUnit.pos.y]);
                Debug.Log("Show!!");
            }
        } else {
            lineRenderer.enabled = false;
            canvas.enabled = false;
            lineRenderer.positionCount = 0;
            numberOfMoves.text = "0";
            Debug.Log("Hide");
        }
        if (path != null && oldPositions != path) {
            if (path.Count > 0) {
                oldPositions = path;
                lineRenderer.positionCount = path.Count;
                lineRenderer.SetPositions(TilesToWorldPositions(path));
                numberOfMoves.text = (path.Count - 2).ToString();
                int midIndex = Mathf.RoundToInt((path.Count - 1) / 2);
                canvas.transform.position = new Vector3(path[midIndex].gameObject.transform.position.x, 2, path[midIndex].gameObject.transform.position.z);
            }
        }
    }

    Vector3[] TilesToWorldPositions(List<Tile> tiles) {
        Vector3[] positions = new Vector3[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) {
            positions[i] = GridUtilities.TileToWorldPos(tiles[i].pos);
        }
        return positions;
    }
}
