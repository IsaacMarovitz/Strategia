using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {Sea, Plains, Swamp, Mountains, Trees, City, CostalCity};

[System.Serializable]
public class Tile {
    public Vector2Int index;
    public TileType tileType;
    public GameObject gameObject;
    public Tile(TileType _tileType, GameObject _gameObject, Vector2Int _index) {
        tileType = _tileType;
        gameObject = _gameObject;
        index = _index;
    }
}
