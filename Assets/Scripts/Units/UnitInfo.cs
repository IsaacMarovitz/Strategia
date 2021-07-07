using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Strategia/UnitInfo")]
public class UnitInfo : ScriptableObject {
    public List<UnitData> landUnits = new List<UnitData>();
    public List<UnitData> airUnits = new List<UnitData>();
    public List<UnitData> navalUnits = new List<UnitData>();
    public List<UnitData> allUnits {
        get {
            return landUnits.Concat(airUnits).Concat(navalUnits).ToList();
        }
    }
}

[System.Serializable]
public struct UnitData {
    public string name;
    public int turnsToCreate;
    public GameObject prefab;
    public Sprite unitIcon;
    public List<TileType> blockedTileTypes;
}