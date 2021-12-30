using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : TurnBehaviour {

    public PlayerTurnStage playerTurnStage = PlayerTurnStage.Waiting;
    public bool hasDied = false;
    public List<City> playerCities;
    public List<Unit> playerUnits;
    [HideInInspector]
    public GameMode gameMode;
    public Color playerColor;
    public Material baseMaterial;
    public CameraController cameraController;
    public Country country;
    public List<Unit> unitQueue;
    public Action cityDataChangedDelegate;

    private bool revealAllTiles = false;

    public List<string> cityNames;
    public Texture2D minimapTexture;
    public Texture2D fogOfWarTexture;
    private FogOfWarState[,] internalFogOfWarMatrix;
    public FogOfWarState[,] fogOfWarMatrix;

    private Material _playerMaterial;
    public Material playerMaterial {
        get {
            if (_playerMaterial == null) {
                _playerMaterial = new Material(baseMaterial);
                _playerMaterial.color = playerColor;
            }

            return _playerMaterial;
        }
    }

    public override void OnUnitAction() {
        if (playerTurnStage == PlayerTurnStage.Complete) {
            foreach (var unit in playerUnits) {
                if (unit.unitTurnStage == UnitTurnStage.Started) {
                    Tank tank = unit as Tank;
                    Fighter fighter = unit as Fighter;

                    if (tank != null) {
                        if (tank.transport != null) {
                            return;
                        }
                    } else if (fighter != null) {
                        if (fighter.carrier != null) {
                            return;
                        }
                    }

                    playerTurnStage = PlayerTurnStage.Started;
                }
            }
        }
    }

    public void UpdateFogOfWar() {
        if (internalFogOfWarMatrix == null || fogOfWarMatrix == null) {
            internalFogOfWarMatrix = new FogOfWarState[tileGrid.width, tileGrid.height];
            fogOfWarMatrix = new FogOfWarState[tileGrid.width, tileGrid.height];
            fogOfWarTexture = new Texture2D(tileGrid.width, tileGrid.height);
            fogOfWarTexture.filterMode = FilterMode.Point;
            minimapTexture = new Texture2D(tileGrid.width, tileGrid.height);
            minimapTexture.filterMode = FilterMode.Point;
            for (int x = 0; x < tileGrid.width; x++) {
                for (int y = 0; y < tileGrid.height; y++) {
                    internalFogOfWarMatrix[x, y] = FogOfWarState.Hidden;
                    fogOfWarMatrix[x, y] = FogOfWarState.Hidden;
                }
            }
        }
        for (int x = 0; x < tileGrid.width; x++) {
            for (int y = 0; y < tileGrid.height; y++) {
                if (internalFogOfWarMatrix[x, y] == FogOfWarState.Visible) {
                    internalFogOfWarMatrix[x, y] = FogOfWarState.Revealed;
                }
            }
        }
        foreach (var unit in playerUnits) {
            List<Tile> revealedTiles = GridUtilities.RadialSearch(unit.pos, 5);
            foreach (var tile in revealedTiles) {
                internalFogOfWarMatrix[tile.pos.x, tile.pos.y] = FogOfWarState.Visible;
            }
        }
        foreach (var city in playerCities) {
            List<Tile> revealedTiles = GridUtilities.RadialSearch(city.pos, 5);
            foreach (var tile in revealedTiles) {
                internalFogOfWarMatrix[tile.pos.x, tile.pos.y] = FogOfWarState.Visible;
            }
        }

        if (!revealAllTiles) {
            fogOfWarMatrix = (FogOfWarState[,])internalFogOfWarMatrix.Clone();
        } else {
            for (int x = 0; x < tileGrid.width; x++) {
                for (int y = 0; y < tileGrid.height; y++) {
                    fogOfWarMatrix[x, y] = FogOfWarState.Visible;
                }
            }
        }

        GenerateTexture();
    }

    public void GenerateTexture() {
        for (int x = 0; x < tileGrid.width; x++) {
            for (int y = 0; y < tileGrid.height; y++) {
                if (fogOfWarMatrix[x, y] == FogOfWarState.Visible) {
                    fogOfWarTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
                    if (grid[x, y].cityOfInfluence.player != null) {
                        minimapTexture.SetPixel(x, y, grid[x, y].cityOfInfluence.player.playerColor);
                    } else {
                        minimapTexture.SetPixel(x, y, Color.grey);
                    }
                } else if (fogOfWarMatrix[x, y] == FogOfWarState.Hidden) {
                    fogOfWarTexture.SetPixel(x, y, Color.black);
                    minimapTexture.SetPixel(x, y, Color.black);
                } else if (fogOfWarMatrix[x, y] == FogOfWarState.Revealed) {
                    fogOfWarTexture.SetPixel(x, y, new Color(1, 1, 1, 0.5f));
                    if (grid[x, y].cityOfInfluence.player != null) {
                        minimapTexture.SetPixel(x, y, grid[x, y].cityOfInfluence.player.playerColor);
                    } else {
                        minimapTexture.SetPixel(x, y, Color.grey);
                    }
                } else {
                    fogOfWarTexture.SetPixel(x, y, Color.red);
                }
            }
        }
        fogOfWarTexture.Apply();
        minimapTexture.Apply();
        if (playerTurnStage == PlayerTurnStage.Started) {
            gameManager.fogOfWarRenderer.material.mainTexture = fogOfWarTexture;
        }
    }


    public void NewDay() {
        unitQueue?.Clear();
        unitQueue = new List<Unit>(playerUnits);
        
        //Debug.Log($"<b>{this.gameObject.name}:</b> Received New Day");
        foreach (var unit in playerUnits) {
            unit.NewDay(this);
        }

        playerTurnStage = PlayerTurnStage.Waiting;
    }

    public void AddUnit(Unit unit) {
        unit.gameObject.name = "Unit " + (playerUnits.Count + 1) + ", " + this.gameObject.name;
        unit.SetColor(this);
        playerUnits.Add(unit);
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn started");
        playerTurnStage = PlayerTurnStage.Started;
        if (playerUnits != null) {
            UpdateFogOfWar();
        }
        if (unitQueue.Count > 0) {
            Unit nextUnit = unitQueue[0];
            UIData.SetUnit(nextUnit, false);
            nextUnit.StartTurn();
            // Prevents Camera Controller from sometimes defocusing unit because of Next Player UI Button press
            cameraController.didClickUI = true;
        } else {
            TurnEnded();
        }
        DelegateManager.playerTurnStartDelegate?.Invoke(this);
        cityDataChangedDelegate?.Invoke();
    }

    public void NextUnit(Unit unit, bool movingLater, bool startNextUnitTurn = true) {
        if (movingLater) {
            unitQueue.Remove(unit);
            unitQueue.Add(unit);
        } else {
            unitQueue.Remove(unit);
        }
        if (unitQueue.Count > 0) {
            if (startNextUnitTurn) {
                Debug.Log($"<b>{this.gameObject.name}:</b> Starting next unit turn");
                Unit nextUnit = unitQueue[0];
                UIData.SetUnit(nextUnit);
                nextUnit.StartTurn();
            }
        } else {
            EndTurnButton();
        }
    }

    public void EndTurnButton() {
        playerTurnStage = PlayerTurnStage.Complete;
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn complete");
        DelegateManager.playerTurnCompleteDelegate?.Invoke(this);
    }

    public void TurnEnded() {
        playerTurnStage = PlayerTurnStage.Ended;
        UIData.SetUnit(null);
        UIData.SetCity(null);
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn ended");
        foreach (var unit in playerUnits) {
            unit.MoveAlongSetPath();
        }
        DelegateManager.playerTurnEndDelegate?.Invoke(this);
        gameManager.NextPlayer();
    }

    public Unit GetCurrentUnit() {
        if (unitQueue.Count > 0) {
            return unitQueue[0];
        }
        return null;
    }

    public bool HasDied() {
        if (playerUnits.Count <= 0 && playerCities.Count <= 0) {
            Debug.Log($"<b>GameManager:</b> Player {gameManager.currentPlayerIndex} has died!");
            hasDied = true;
            return true;
        }
        return false;
    }

    public string AssignName() {
        if (cityNames.Count > 0) {
            string returnName = cityNames[0];
            cityNames.RemoveAt(0);
            return returnName;
        } else {
            for (int i = 0; i < country.names.Length; i++) {
                country.names[i] = "New " + country.names[i];
                cityNames.Add(country.names[i]);
            }
            cityNames = tileGrid.FisherYates(cityNames);
            string returnName = cityNames[0];
            cityNames.RemoveAt(0);
            return returnName;
        }
    }

    public void RevealAllTiles(bool value) {
        revealAllTiles = value;
        UpdateFogOfWar();
    }

    public void SpawnUnit(Vector2Int pos, UnitType unitType) {
        Unit unitFromType = GameManager.Instance.GetUnitFromType(unitType);
        if (unitFromType == null) { return; }

        Unit unit = GameObject.Instantiate(unitFromType.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
        unit.gameObject.transform.position = GridUtilities.TileToWorldPos(pos, unit.yOffset);
        unit.pos = pos;
        unit.gameObject.transform.parent = this.gameObject.transform;
        unit.player = this;
        AddUnit(unit);
        grid[pos.x, pos.y].unitOnTile = unit;

        if (grid[pos.x, pos.y].isCityTile) {
            unit.unitAppearanceManager.Hide();
            unit.oldCity = grid[pos.x, pos.y].gameObject.GetComponent<City>();    
            grid[pos.x, pos.y].gameObject.GetComponent<City>().AddUnit(unit);
        }
    }
}

public enum FogOfWarState { Visible, Revealed, Hidden }
public enum PlayerTurnStage { Waiting, Started, Complete, Ended }
