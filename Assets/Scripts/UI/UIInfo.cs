using UnityEngine;

[CreateAssetMenu(fileName = "UIInfo", menuName = "Strategia/UIInfo")]
public class UIInfo : ScriptableObject {
    public bool unitSelected = false;
    public Vector3 worldPos;
    public Vector2Int pos = new Vector2Int(0,0);
    public int movesLeft = 0;
    public bool[] moveDirs = new bool[8];
    public int day = 1;
    public int dir = 1;
    public bool newMove = false;
}
