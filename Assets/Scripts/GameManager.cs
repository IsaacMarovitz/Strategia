using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public UIInfo UIInfo;
    public int day = 1;
    public List<Player> playerList;
    public bool dayStarted = false;

    private int currentPlayerIndex = 0;

    void Update() {
        // Start player turns
        if (!dayStarted) {
            if (!playerList[currentPlayerIndex].turnCompleted) {
                TakeTurn();
            } else {
                Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn has completed");
                playerList[currentPlayerIndex].turnCompleted = false;
                playerList[currentPlayerIndex].turnStarted = false;
                if (currentPlayerIndex < (playerList.Count - 1)) {
                    currentPlayerIndex++;
                    TakeTurn();
                } else {
                    Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn has completed");
                    Debug.Log("Day Complete");
                    currentPlayerIndex = 0;
                    BeginNextDay();
                }
            }
        }

        UIInfo.day = day;
    }

    public void BeginNextDay() {
        day++;
        dayStarted = true;
        Debug.Log("Next Day Started");
    }

    public void TakeTurn() {
        if (!playerList[currentPlayerIndex].turnStarted) {
            playerList[currentPlayerIndex].turnStarted = true;
            playerList[currentPlayerIndex].TakeTurn();
            Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn has started");
        }
    }
}
