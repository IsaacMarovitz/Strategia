using System;
using System.Collections.Generic;
using UnityEngine;
using Grid = Strategia.Grid;

public class GameManager : MonoBehaviour {

    public UIInfo UIInfo;
    public int day = 0;
    public List<Player> playerList;
    public bool dayCompleted = true;
    public GameMode gameMode = GameMode.LocalMultiplayer;
    public Grid grid;
    public GameInfo gameInfo;
    /*public int numberOfPlayers;
    public GameObject playerPrefab;
    public Transform playerParent;*/
    public Action newDayDelegate;

    private int currentPlayerIndex = 0;

    public void Start() {
        // Recieve game information from GameInfo ScriptableObject at the same time the Grid is being generated with appropriate settings
        gameMode = gameInfo.gameMode;
        //numberOfPlayers = gameInfo.numberOfPlayers;

        // Create players in the appropriate game mode
        /*for (int i = 0; i < numberOfPlayers; i++) {
            CreatePlayer(i+1);            
        }*/

        // Asign player starting cities
        foreach (var player in playerList) {
            player.playerCities.Add(grid.ChoosePlayerCity());
            player.InitaliseStartCity();
        }
        
        // Start Game
        NewDay();
    }

    public void NewDay() {
        if (dayCompleted) {
            dayCompleted = false;
            day++;
            UIInfo.day = day;
            foreach (var player in playerList) {
                player.NewDay(this);
            }
            currentPlayerIndex = 0;
            playerList[currentPlayerIndex].StartTurn();
        }
    }

    public void NextPlayer() {
        currentPlayerIndex++;
        if (currentPlayerIndex < playerList.Count) {
            playerList[currentPlayerIndex].StartTurn();
        } else {
            DayComplete();
        }
    }

    public void DayComplete() {
        dayCompleted = true;
        newDayDelegate?.Invoke();
    }

    /*public void CreatePlayer(int playerIndex) {    
        GameObject instantiatedPlayer = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        instantiatedPlayer.transform.parent = playerParent;
        instantiatedPlayer.transform.name = "Player " + playerIndex;
        Player player = instantiatedPlayer.GetComponent<Player>();
        player.gameMode = gameMode;
        playerList.Add(player);
    }*/
}

public enum GameMode { SinglePlayer, LocalMultiplayer, OnlineMultiplayer };
