using UnityEngine;
using System.Collections.Generic;

public class TileScript : MonoBehaviour {

    public Visibility visibility = Visibility.Undiscovered;
    public TileType tileType;
    public Material undiscoveredMaterial;
    public Material hiddenMaterial;
    public Material visibleMaterial;
    public GameObject plane;
    public MeshRenderer planeMeshRenderer;
    public MeshRenderer meshRenderer;
    public Tile tile;
    public List<GameObject> children;
    public bool isOwnedByCity;
    public Unit unitOnTile;

    public void Start() {
        UpdateTile();
    }

    public void UpdateTile() {
        switch (visibility) {
            case Visibility.Undiscovered:
                foreach (var child in children) {
                    child.SetActive(false);
                }
                plane.SetActive(true);
                planeMeshRenderer.material = undiscoveredMaterial;
                meshRenderer.enabled = false;
                break;
            case Visibility.Hidden:
                foreach (var child in children) {
                    child.SetActive(true);
                }
                plane.SetActive(false);
                planeMeshRenderer.material = null;
                if (tileType != TileType.Sea) {
                    meshRenderer.enabled = true;
                    meshRenderer.material = hiddenMaterial;
                }
                break;
            case Visibility.Visable:
                foreach (var child in children) {
                    child.SetActive(true);
                }
                plane.SetActive(false);
                planeMeshRenderer.material = null;
                if (tileType != TileType.Sea) {
                    meshRenderer.enabled = true;
                    meshRenderer.material = visibleMaterial;
                }
                break;
        }
    }

    public void ChangeVisibility(Visibility _visibility) {
        if (!isOwnedByCity) {
            visibility = _visibility;
            UpdateTile();
        }
    }
    public bool SetUnit(Unit unit) {
        unitOnTile = unit;
        if (tileType == TileType.City || tileType == TileType.CostalCity) {
            return true;
        } else {
            return false;
        }
    }
}

public enum Visibility { Undiscovered, Hidden, Visable };