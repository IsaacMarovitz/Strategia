using UnityEngine;

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

    public void Start() {
        for (int i = 0; i < this.transform.childCount; i++) {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        plane.SetActive(true);
        planeMeshRenderer.material = undiscoveredMaterial;
        meshRenderer.enabled = false;
    }

    public void UpdateTile() {
        switch (visibility) {
            case Visibility.Undiscovered:
                for (int i = 0; i < this.transform.childCount; i++) {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
                plane.SetActive(true);
                planeMeshRenderer.material = undiscoveredMaterial;
                meshRenderer.enabled = false;
                break;
            case Visibility.Hidden:
                for (int i = 0; i < this.transform.childCount; i++) {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                }
                plane.SetActive(false);
                planeMeshRenderer.material = null;
                if (tileType != TileType.Sea) {
                    meshRenderer.enabled = true;
                    meshRenderer.material = hiddenMaterial;
                }
                break;
            case Visibility.Visable:
                for (int i = 0; i < this.transform.childCount; i++) {
                    this.transform.GetChild(i).gameObject.SetActive(true);
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
        visibility = _visibility;
        UpdateTile();
    }
}

public enum Visibility { Undiscovered, Hidden, Visable };