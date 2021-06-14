using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CancelMoveUI : MonoBehaviour {

    public LineRenderer lineRenderer;
    public TMP_Text numberOfMoves;
    public Canvas canvas;
    public bool showLine = false;
    public List<Tile> path;

    private List<Tile> oldPositions;
    private Unit oldUnit;

    void Update() {
        if (UIData.Instance.currentUnit != null && showLine) {
            if (UIData.Instance.currentUnit != oldUnit && UIData.Instance.currentUnit.turnStage == TurnStage.PathSet && UIData.Instance.currentUnit.path != null) {
                oldUnit = UIData.Instance.currentUnit;
                lineRenderer.enabled = true;
                canvas.enabled = true;
                canvas.transform.position = GridUtilities.TileToWorldPos(UIData.Instance.currentUnit.pos, 2);
                path = UIData.Instance.currentUnit.path;
            }
        } else {
            lineRenderer.enabled = false;
            canvas.enabled = false;
        }
        if (path != null && oldPositions != path) {
            if (path.Count > 0) {
                oldPositions = path;
                lineRenderer.positionCount = path.Count;
                lineRenderer.SetPositions(TilesToWorldPositions(path));
                numberOfMoves.text = (path.Count - 1).ToString();
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

    public void Show() {
        showLine = true;
    }

    public void Hide() {
        showLine = false;
        lineRenderer.enabled = false;
        canvas.enabled = false;
        lineRenderer.positionCount = 0;
        numberOfMoves.text = "0";
    }
}
