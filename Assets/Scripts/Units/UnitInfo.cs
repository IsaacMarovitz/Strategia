using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Strategia/UnitInfo")]
public class UnitInfo : ScriptableObject {
    public string unitName { get { return _unitName; } }
    [SerializeField] private string _unitName;
    public int maxHealth { get { return _maxHealth; } }
    [SerializeField] private int _maxHealth;
    public int maxMoves { get { return _maxMoves; } }
    [SerializeField] private int _maxMoves;
    public float yOffset { get { return _yOffset; } }
    [SerializeField] private float _yOffset;
    public int turnsToCreate { get { return _turnsToCreate; } }
    [SerializeField] private int _turnsToCreate;

    public UnitType unitType { get { return _unitType; } }
    [SerializeField] private UnitType _unitType;
    public Sprite unitIcon { get { return _unitIcon; } }
    [SerializeField] private Sprite _unitIcon;
    public List<TileType> blockedTileTypes { get { return _blockedTileTypes; } }
    [SerializeField] private List<TileType> _blockedTileTypes;
}