using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {Sea, Plains, Swamp, Mountains, Trees, City, CostalCity};

[System.Serializable]
public class Tile {
    public TileType tileType;
    public GameObject gameObject;
    public Tile(TileType _tileType, GameObject _gameObject) {
        tileType = _tileType;
        gameObject = _gameObject;
    }
}
