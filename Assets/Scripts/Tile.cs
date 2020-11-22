using UnityEngine;

public enum TileType {Sea, Plains, Swamp, Mountains, Trees, City, CostalCity};

[System.Serializable]
public class Tile {
    public Vector2Int index;
    public TileType tileType;
    public GameObject gameObject;
    public int islandIndex;
    public Unit unitOnTile;

    public Tile(TileType _tileType, GameObject _gameObject, Vector2Int _index, int _islandIndex) {
        tileType = _tileType;
        gameObject = _gameObject;
        index = _index;
        islandIndex = _islandIndex;
    }
}
