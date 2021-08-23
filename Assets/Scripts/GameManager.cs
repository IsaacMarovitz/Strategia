using System;
using System.Collections.Generic;
using UnityEngine;
using TileGrid = Strategia.TileGrid;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameMode gameMode = GameMode.LocalMultiplayer;
    public CameraController cameraController;
    public TileGrid tileGrid;
    public GameInfo gameInfo;
    public UnitInfo unitInfo;
    public MeshRenderer fogOfWarRenderer;

    public int numberOfPlayers;
    public GameObject playerPrefab;
    public Transform playerParent;

    public int day = 0;
    public int currentPlayerIndex = 0;
    public List<Player> playerList;
    public bool dayCompleted = true;

    [SerializeField]
    private City currentCity;
    [SerializeField]
    private Unit currentUnit;
    [SerializeField]
    private Tile mouseOverTile;

    public Action dayEndedDelegate;
    public Action newDayDelegate;
    public Action nextPlayerDelegate;
    public Action pauseGame;
    public Action resumeGame;
    public Action<bool> fastProdDelegate;

    private float hueOffset;
    private List<Country> countries = new List<Country>();

    [HideInInspector]
    public bool infiniteAttack = false;

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
        countries = tileGrid.FisherYates(countries);

        // Create players in the appropriate game mode
        for (int i = 0; i < numberOfPlayers; i++) {
            CreatePlayer(i + 1);
        }

        // Asign player starting cities
        foreach (Player player in playerList) {
            Debug.Log("<b>Game Manager:</b> Choosing " + player.gameObject.name + " starting city");
            tileGrid.ChoosePlayerCity().StartGame(player);
        }

        // Start Game
        NewDay();
    }

    public void Update() {
        currentCity = UIData.currentCity;
        currentUnit = UIData.currentUnit;
        mouseOverTile = UIData.mouseOverTile;
    }

    public void NewDay() {
        if (dayCompleted) {
            Debug.Log("<b>GameManager:</b> Starting New Day");
            newDayDelegate?.Invoke();
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
        dayEndedDelegate?.Invoke();
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

        if (playerIndex - 1 < countries.Count) {
            player.country = countries[playerIndex - 1];
        } else {
            player.country = CityNames.Overflow;
        }

        for (int i = 0; i < player.country.names.Length; i++) {
            player.cityNames.Add(player.country.names[i]);
        }
        player.cityNames = tileGrid.FisherYates(player.cityNames);


        playerList.Add(player);
    }

    public Player GetCurrentPlayer() {
        return playerList[currentPlayerIndex - 1];
    }

    public void OnPlayerTurnStart(Player player) {
        DelegateManager.playerTurnStartDelegate?.Invoke(player);
    }

    public void OnPlayerTurnEnd(Player player) {
        DelegateManager.playerTurnEndDelegate?.Invoke(player);
    }

    public void OnUnitTurnStart(Unit unit) {
        DelegateManager.unitTurnStartDelegate?.Invoke(unit);
    }

    public void OnUnitMove(Unit unit) {
        DelegateManager.unitMoveDelegate?.Invoke(unit);
    }

    public void OnFogOfWarUpdate(Player player) {
        DelegateManager.fogOfWarUpdateDelegate?.Invoke(player);
    }
}

public enum GameMode { SinglePlayer, LocalMultiplayer, OnlineMultiplayer };
