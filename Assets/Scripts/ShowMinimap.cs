using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShowMinimap : MonoBehaviour {

    public RawImage image;
    public List<Vector2> cityPos = new List<Vector2>();

    void Start() {
        foreach (var tile in GameManager.Instance.grid.grid) {
            if (tile.tileType == TileType.City || tile.tileType == TileType.CostalCity) {
                cityPos.Add(tile.index);
            }
        }
        image.texture = Voronoi.GenerateVoronoi(cityPos);
    }
}
