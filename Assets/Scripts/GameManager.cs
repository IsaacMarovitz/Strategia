using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileGrid = Strategia.TileGrid;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public int day = 0;
    public List<Player> playerList;
    public bool dayCompleted = true;
    public GameMode gameMode = GameMode.LocalMultiplayer;
    public TileGrid grid;
    public GameInfo gameInfo;
    public UnitInfo unitInfo;
    public int numberOfPlayers;
    public GameObject playerPrefab;
    public Transform playerParent;
    public Action newDayDelegate;
    public Action nextPlayerDelegate;
    public Action pauseGame;
    public Action resumeGame;
    public MeshRenderer fogOfWarTexture;
    public int currentPlayerIndex = 0;
    public CameraController cameraController;

    [HideInInspector]
    public bool fastProd = false;

    private float hueOffset;
    private List<Country> countries = new List<Country>();

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(_instance);
    }

    public void Start() {
        // Recieve game information from GameInfo ScriptableObject at the same time the Grid is being generated with appropriate settings
        gameMode = gameInfo.gameMode;
        numberOfPlayers = gameInfo.numberOfPlayers;
        hueOffset = 1f / numberOfPlayers;

        for (int i = 0; i < CityNames.countries.Length; i++) {
            countries.Add(CityNames.countries[i]);
        }
        countries = grid.FisherYates(countries);

        // Create players in the appropriate game mode
        for (int i = 0; i < numberOfPlayers; i++) {
            CreatePlayer(i + 1);
        }

        // Asign player starting cities
        foreach (Player player in playerList) {
            Debug.Log("<b>Game Manager:</b> Choosing " + player.gameObject.name + " starting city");
            City returnedCity = grid.ChoosePlayerCity();
            player.InitaliseStartCity(returnedCity);
            StartCoroutine(Wait());
        }

        // Start Game
        NewDay();
    }

    public IEnumerator Wait() {
        yield return new WaitForSeconds(2);
    }

    public void NewDay() {
        if (dayCompleted) {
            Debug.Log("<b>GameManager:</b> Starting New Day");
            dayCompleted = false;
            day++;
            if (day > 1000) {
                throw new System.Exception("Day Count Surpassed Limit!");
            }
            foreach (var player in playerList) {
                player.NewDay();
            }
            currentPlayerIndex = 0;
            NextPlayer();
        }
    }

    public void NextPlayer() {
        if (currentPlayerIndex < playerList.Count) {
            //Debug.Log("<b>GameManager:</b> Starting Player " + (currentPlayerIndex+1) + "'s turn");
            cameraController.NextPlayer();
            currentPlayerIndex++;
            nextPlayerDelegate?.Invoke();
            Player nextPlayer = playerList[currentPlayerIndex - 1];
            if (nextPlayer.HasDied() || nextPlayer.hasDied) {
                NextPlayer();
            } else {
                nextPlayer.StartTurn();
            }
        } else {
            DayComplete();
        }
    }

    public void DayComplete() {
        Debug.Log("<b>GameManager:</b> Day Complete");
        dayCompleted = true;
        newDayDelegate?.Invoke();
    }

    public void Pause() {
        pauseGame?.Invoke();
    }

    public void Resume() {
        resumeGame?.Invoke();
    }

    public void CreatePlayer(int playerIndex) {
        GameObject instantiatedPlayer = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        instantiatedPlayer.transform.parent = playerParent;
        instantiatedPlayer.transform.name = "Player " + playerIndex;
        Player player = instantiatedPlayer.GetComponent<Player>();
        player.gameMode = gameMode;
        player.playerColor = Color.HSVToRGB(playerIndex * hueOffset, 1f, 0.7f);
        player.cameraController = cameraController;

        if (playerIndex-1 < countries.Count) {
            player.country = countries[playerIndex-1];
        } else {
            player.country = CityNames.Overflow;
        }

        for (int i = 0; i < player.country.names.Length; i++) {
            player.cityNames.Add(player.country.names[i]);
        }
        player.cityNames = grid.FisherYates(player.cityNames);
            

        playerList.Add(player);
    }

    public void UpdateFogOfWarObjects(float[,] fogOfWarArray) {
        for (int x = 0; x < grid.width; x++) {
            for (int y = 0; y < grid.height; y++) {
                if (fogOfWarArray[x, y] == 0) {
                    foreach (Transform child in grid.grid[x, y].gameObject.transform) {
                        child.gameObject.SetActive(false);
                    }
                } else {
                    foreach (Transform child in grid.grid[x, y].gameObject.transform) {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public Player GetCurrentPlayer() {
        return playerList[currentPlayerIndex - 1];
    }
}

public enum GameMode { SinglePlayer, LocalMultiplayer, OnlineMultiplayer };
