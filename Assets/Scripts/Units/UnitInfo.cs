using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Strategia/UnitInfo")]
public class UnitInfo : ScriptableObject {
    public string unitName;
    public int maxHealth;
    public int maxMoves;
    public float yOffset;
    public int turnsToCreate;

    public UnitType unitType;
    public Sprite unitIcon;
    public List<TileType> blockedTileTypes;
}