using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "Strategia/GameInfo")]
public class GameInfo : ScriptableObject {
    public GameMode gameMode;
    public int numberOfPlayers;
}