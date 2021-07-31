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
                path = new List<Tile>(UIData.Instance.currentUnit.path);
                for (int i = 0; i < path.Count; i++) {
                    if (UIData.Instance.currentUnit.player.fogOfWarMatrix[path[i].pos.x, path[i].pos.y] != FogOfWarState.Hidden) {
                        if (UIData.Instance.currentUnit.CheckDir(path[i]) == TileMoveStatus.Blocked) {
                            path.RemoveRange(i, path.Count - i);
                            break;
                        }
                    }
                }
                path.Insert(0, UIData.Instance.currentUnit.currentTile);
            } else {
                lineRenderer.enabled = false;
                canvas.enabled = false;
                lineRenderer.positionCount = 0;
                numberOfMoves.text = "0";
            }
        } else {
            lineRenderer.enabled = false;
            canvas.enabled = false;
            lineRenderer.positionCount = 0;
            numberOfMoves.text = "0";
        }
        if (path != null && oldPositions != path) {
            if (path.Count > 0) {
                oldPositions = path;
                lineRenderer.positionCount = path.Count;
                lineRenderer.SetPositions(GridUtilities.TilesToWorldPos(path));
                numberOfMoves.text = (path.Count - 2).ToString();
                int midIndex = Mathf.RoundToInt((path.Count - 1) / 2);
                canvas.transform.position = new Vector3(path[midIndex].gameObject.transform.position.x, 2, path[midIndex].gameObject.transform.position.z);
            }
        }
    }
}
