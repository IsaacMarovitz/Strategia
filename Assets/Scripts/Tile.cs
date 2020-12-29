using UnityEngine;

public enum TileType { Sea, Plains, Swamp, Mountains, Trees, City, CostalCity };

[System.Serializable]
public class Tile : IHeapItem<Tile> {
    public Vector2Int pos;
    public TileType tileType;
    public GameObject gameObject;
    public int islandIndex;
    public Unit unitOnTile;
    public City cityOfInfluence;

    public bool walkable;
    public int gCost;
    public int hCost;
    public Tile parent;
    int heapIndex;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Tile nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    public Tile(bool walkable, Vector2Int pos) {
        this.walkable = walkable;
        this.pos = pos;
    }

    public Tile(TileType _tileType, GameObject _gameObject, Vector2Int _index, int _islandIndex) {
        tileType = _tileType;
        gameObject = _gameObject;
        pos = _index;
        islandIndex = _islandIndex;
    }
}
