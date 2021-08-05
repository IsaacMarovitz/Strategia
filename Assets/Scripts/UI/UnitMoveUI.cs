using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitMoveUI : TurnBehaviour {

    public Unit unit;
    public LineRenderer lineRenderer;
    public GameObject tileSelector;
    public MeshRenderer tileSelectorMeshRenderer;
    public TMP_Text numberOfTurns;
    public Canvas canvas;
    public Player currentPlayer;
    public List<Tile> path = new List<Tile>();

    public bool isSelected = false;
    public bool isMoving = false;

    public Material hiddenTRMaterial;
    public Material moveTRMaterial;
    public Material blockedTRMaterial;
    public Material setPathTRMaterial;

    public Material hiddenLRMaterial;
    public Material moveLRMaterial;
    public Material setPathLRMaterial;

    private Tile oldMouseOverTile;
    private bool isPathHiden = false;

    void Update() {
        if (currentPlayer != unit.player) {
            Hide();
            return;
        }

        isSelected = UIData.Instance.currentUnit == unit;

        if (unit.turnStage == TurnStage.PathSet) {
            path = new List<Tile>(unit.path);
            path.Insert(0, unit.currentTile);
            
            foreach (var tile in path) {
                if (currentPlayer.fogOfWarMatrix[tile.pos.x, tile.pos.y] != FogOfWarState.Visible) {
                    isPathHiden = true;
                }
            }

            tileSelector.SetActive(true);
            tileSelector.transform.position = GridUtilities.TileToWorldPos(path[path.Count - 1].pos);

            lineRenderer.enabled = true;
            lineRenderer.positionCount = path.Count;
            lineRenderer.SetPositions(TilesToWorldPositions(path));

            numberOfTurns.text = (path.Count - 1).ToString();
            int midIndex = Mathf.RoundToInt((path.Count - 1) / 2);
            Vector3 midIndexPos = path[midIndex].gameObject.transform.position;
            canvas.transform.position = new Vector3(midIndexPos.x, 2, midIndexPos.z);

            if (isSelected) {
                // Show set path with bright material
                canvas.enabled = true;
                if (isPathHiden) {
                    tileSelectorMeshRenderer.material = hiddenTRMaterial;
                    lineRenderer.material = hiddenLRMaterial;
                } else {
                    tileSelectorMeshRenderer.material = moveTRMaterial;
                    lineRenderer.material = moveLRMaterial;
                }
            } else {
                // Show set path with more muted colours
                canvas.enabled = false;
                tileSelectorMeshRenderer.material = setPathTRMaterial;
                lineRenderer.material = setPathLRMaterial;
            }
        } else {
            if (isSelected && isMoving) {
                MoveSelector();
                return;
            }
            Hide();
        }
    }

    void MoveSelector() {
        Tile mouseOverTile = UIData.Instance.mouseOverTile;
        if (mouseOverTile == null || mouseOverTile == oldMouseOverTile) { return; }

        oldMouseOverTile = mouseOverTile;
        tileSelector.SetActive(true);
        tileSelector.transform.position = GridUtilities.TileToWorldPos(mouseOverTile.pos);

        if (unit.blockedTileTypes.Contains(mouseOverTile.tileType) && currentPlayer.fogOfWarMatrix[mouseOverTile.pos.x, mouseOverTile.pos.y] != FogOfWarState.Hidden) {
            tileSelectorMeshRenderer.material = blockedTRMaterial;
            lineRenderer.enabled = false;
            canvas.enabled = false;
            path = null;
        } else {
            path = GridUtilities.FindPath(unit.currentTile, mouseOverTile, out bool goesThroughHiddenTiles);
            if (path == null) {
                // Path is blocked
                tileSelectorMeshRenderer.material = blockedTRMaterial;
                lineRenderer.enabled = false;
                canvas.enabled = false;
            } else {
                if (goesThroughHiddenTiles) {
                    tileSelectorMeshRenderer.material = hiddenTRMaterial;
                    lineRenderer.material = hiddenLRMaterial;
                } else {
                    tileSelectorMeshRenderer.material = moveTRMaterial;
                    lineRenderer.material = moveLRMaterial;
                }

                Show();
                if (path.Count > 0) {
                    lineRenderer.positionCount = path.Count;
                    lineRenderer.SetPositions(TilesToWorldPositions(path));
                    // Change this later
                    numberOfTurns.text = (path.Count - 1).ToString();
                    int midIndex = Mathf.RoundToInt((path.Count - 1) / 2);
                    Vector3 midIndexPos = path[midIndex].gameObject.transform.position;
                    canvas.transform.position = new Vector3(midIndexPos.x, 2, midIndexPos.z);
                }
            }
        }
    }

    public override void OnFogOfWarUpdate(Player player) {
        currentPlayer = player;
    }

    Vector3[] TilesToWorldPositions(List<Tile> tiles) {
        Vector3[] positions = new Vector3[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) {
            positions[i] = GridUtilities.TileToWorldPos(tiles[i].pos);
        }
        return positions;
    }

    void Show() {
        // Show everything
        tileSelector.SetActive(true);
        lineRenderer.enabled = true;
        canvas.enabled = true;
    }

    void Hide() {
        // Hide everything
        tileSelector.SetActive(false);
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        canvas.enabled = false;
        numberOfTurns.text = "0";
    }

    public bool Move() {
        if (path != null) {
            unit.MoveAlongPath(path);
            return true;
        } else {
            return false;
        }
    }

    public void MoveButtonSelected() => isMoving = true;

    public void MoveButtonDeselected() => isMoving = false;
}