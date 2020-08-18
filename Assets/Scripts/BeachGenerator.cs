using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachGenerator : MonoBehaviour {

    public Strategia.Grid gridScript;
    public MeshFilter meshFilter;
    public List<Vector3> vertices;

    private Tile[,] grid;
    private List<Tile> costalTiles = new List<Tile>();
    private Mesh mesh;
    private bool slotVertex = false;
    private int vertexSlotIndex;
    public Color32 color = new Color32(0, 0, 0, 255);

    void Start() { 
        grid = gridScript.grid;
        foreach (var tile in grid) {
            if (tile.tileType != TileType.Sea) {
                if (gridScript.CostalCheck(tile.index.x, tile.index.y)) {
                    costalTiles.Add(tile);
                    tile.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
        }
        foreach (var tile in costalTiles) {
            Vector3 tilePos = tile.gameObject.transform.position;
            Vector3 topLeft = new Vector3(tilePos.x - (gridScript.tileWidth/2), tilePos.y, tilePos.z - (gridScript.tileHeight/2));
            Vector3 topRight = new Vector3(tilePos.x + (gridScript.tileWidth/2), tilePos.y, tilePos.z - (gridScript.tileHeight/2));
            Vector3 bottomLeft = new Vector3(tilePos.x - (gridScript.tileWidth/2), tilePos.y, tilePos.z + (gridScript.tileHeight/2));
            Vector3 bottomRight = new Vector3(tilePos.x + (gridScript.tileWidth/2), tilePos.y, tilePos.z + (gridScript.tileHeight/2));
            AddVertex(topLeft);
            AddVertex(topRight);
            AddVertex(bottomLeft);
            AddVertex(bottomRight);
        }
        /*meshFilter.mesh = mesh = new Mesh();
        mesh.name = "Inverse Beach";
        mesh.vertices = vertices.ToArray();*/
    }

    void AddVertex(Vector3 vertex) {
        if (!vertices.Contains(vertex)) {
            if (slotVertex) {
                slotVertex = false;
                vertices.Insert(vertexSlotIndex, vertex);
            } else {
                vertices.Add(vertex);
            }
        } else {
            vertexSlotIndex = vertices.IndexOf(vertex) + 1;
            Debug.Log(vertexSlotIndex);
            slotVertex = true;
        }
    }

    void OnDrawGizmos() {
        foreach (var vertex in vertices) {
            Gizmos.DrawSphere(vertex, 0.5f);
            Gizmos.color = (Color)color;
            if (color.r < 255) {
                color.r++;
            } else if (color.g < 255) {
                color.g++;
            } else if (color.b < 255) {
                color.b++;
            } else {
            }
        }
    }
}
