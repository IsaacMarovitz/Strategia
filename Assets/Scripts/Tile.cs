using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {Sea, Plains, Swamp, Mountains, Trees, City, CostalCity};

[System.Serializable]
public class Tile {
    public Vector2Int index;
    public TileType tileType;
    public GameObject gameObject;
    public TileScript tileScript;
    public int islandIndex;
    public Tile(TileType _tileType, GameObject _gameObject, Vector2Int _index, TileScript _tileScript, int _islandIndex) {
        tileType = _tileType;
        gameObject = _gameObject;
        index = _index;
        tileScript = _tileScript;
        islandIndex = _islandIndex;
    }
}
