using UnityEngine;

[CreateAssetMenu(fileName = "UIInfo", menuName = "Strategia/UIInfo")]
public class UIInfo : ScriptableObject {
    public int day = 1;

    public Unit unit;
    public int dir = 1;
    public bool newMove = false;

    public City city;
}
