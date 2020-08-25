using System.Collections.Generic;
using UnityEngine;

namespace Strategia {
    public class Grid : MonoBehaviour {

        public int width = 10;
        public int height = 10;
        public float tileWidth;
        public float tileHeight;
        public float falloffA = 3f;
        public float falloffB = 2.2f;
        public float scale = 7f;
        public int numberOfCostalCities;
        public int numberofCities;
        public Tile[,] grid;
        [Range(0, 10000)]
        public int seed;
        public int islandCount;
        public int minimumIslandArea;
        public Transform tileParent;
        // Sea, Plains, Swamp, Mountains, Trees, City, Costal City
        public GameObject[] prefabs = new GameObject[7];

        private List<List<Tile>> islandList = new List<List<Tile>>();

        void Awake() {
            CreateGrid();
        }

        // Delete previous grid and then calculate new terrain based off of noise value
        public void CreateGrid() {
            DeleteGrid();
            seed = Random.Range(0, 10000);
            grid = new Tile[width, height];
            float[,] noiseMap = PerlinNoise.CalculateNoise(width, height, seed, scale, falloffA, falloffB);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    grid[x, y] = new Tile(TileType.Sea, null, new Vector2Int(x, y), null, 0);

                    float sample = noiseMap[x, y];

                    if ((sample >= 0) && (sample < 0.5)) {
                        grid[x, y].tileType = TileType.Sea;
                    } else if ((sample > 0.5) && (sample < 0.6)) {
                        grid[x, y].tileType = TileType.Plains;
                    } else if ((sample > 0.6) && (sample < 0.61)) {
                        grid[x, y].tileType = TileType.Swamp;
                    } else if ((sample > 0.73) && (sample < 0.85)) {
                        grid[x, y].tileType = TileType.Trees;
                    } else if (sample > 0.85) {
                        grid[x, y].tileType = TileType.Mountains;
                    } else {
                        grid[x, y].tileType = TileType.Plains;
                    }
                }
            }
            CalculateIslands();
            CalculateCities();
            SpawnTiles();
        }

        // Assign an island index to every tile and destroy islands that are too small
        public void CalculateIslands() {
            int islandIndex = 1;
            foreach (var tile in grid) {
                if (tile.islandIndex == 0 && tile.tileType != TileType.Sea) {
                    List<Tile> islandTiles = GridUtilities.FloodFill(grid, tile);
                    islandList.Add(islandTiles);
                    if (islandTiles.Count < minimumIslandArea) {
                        foreach (var islandTile in islandTiles) {
                            islandTile.tileType = TileType.Sea;
                        }
                    } else {
                        foreach (var islandTile in islandTiles) {
                            islandTile.islandIndex = islandIndex;
                        }
                        islandIndex++;
                    }
                }
            }
            islandCount = islandIndex - 1;
        }

        // Perform a Fisher-Yates shuffle on the list to generate cities
        public void CalculateCities() {
            List<Tile> potentialCityTiles = new List<Tile>();
            foreach (var tile in grid) {
                if (tile.tileType == TileType.Plains) {
                    potentialCityTiles.Add(new Tile(TileType.City, null, tile.index, null, tile.islandIndex));
                }
            }
            foreach (var tile in potentialCityTiles) {
                if (GridUtilities.CostalCheck(grid, width, height, tile.index)) {
                    tile.tileType = TileType.CostalCity;
                } 
            }
            int n = potentialCityTiles.Count;
            while (n > 1) {
                n--;
                int k = Random.Range(0, n + 1);
                var value = potentialCityTiles[k];
                potentialCityTiles[k] = potentialCityTiles[n];
                potentialCityTiles[n] = value;
            }
            int calculatedCostalCities = 0;
            int calculatedCities = 0;
            bool outOfCities = false;

            while (!outOfCities) {
                foreach (var city in potentialCityTiles) {
                    if ((city.tileType == TileType.CostalCity) && (calculatedCostalCities < numberOfCostalCities)) {
                        calculatedCostalCities++;
                        grid[city.index.x, city.index.y].tileType = TileType.CostalCity;

                    } else if ((city.tileType == TileType.City) && (calculatedCities < numberofCities)) {
                        calculatedCities++;
                        grid[city.index.x, city.index.y].tileType = TileType.City;

                    }
                }
                outOfCities = true;
            }
            potentialCityTiles.Clear();
        }

        // Instantiate in all tiles
        public void SpawnTiles() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GameObject instantiatedTile;
                    City cityScript;
                    switch (grid[x, y].tileType) {
                        case TileType.Sea:
                            instantiatedTile = GameObject.Instantiate(prefabs[0], new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Plains:
                            instantiatedTile = GameObject.Instantiate(prefabs[1], new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Swamp:
                            instantiatedTile = GameObject.Instantiate(prefabs[2], new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Mountains:
                            instantiatedTile = GameObject.Instantiate(prefabs[3], new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Trees:
                            instantiatedTile = GameObject.Instantiate(prefabs[4], new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.City:
                            instantiatedTile = GameObject.Instantiate(prefabs[5], new Vector3(x * tileWidth, 0, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            instantiatedTile.transform.tag = "City";
                            cityScript = instantiatedTile.GetComponent<City>();
                            cityScript.pos = new Vector2Int(x, y);
                            cityScript.gridScript = this;
                            break;
                        case TileType.CostalCity:
                            instantiatedTile = GameObject.Instantiate(prefabs[6], new Vector3(x * tileWidth, 0, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            instantiatedTile.transform.tag = "City";
                            cityScript = instantiatedTile.GetComponent<City>();
                            cityScript.pos = new Vector2Int(x, y);
                            cityScript.gridScript = this;
                            break;
                        default:
                            instantiatedTile = null;
                            break;
                    }
                    if (instantiatedTile != null) {
                        instantiatedTile.transform.parent = tileParent;
                        instantiatedTile.name = x + ", " + y;
                        grid[x, y].tileScript = instantiatedTile.gameObject.GetComponent<TileScript>();
                        grid[x, y].tileScript.tile = grid[x, y];
                        grid[x, y].gameObject = instantiatedTile;
                    }
                }
            }
        }

        // Delete the grid and clear lists
        public void DeleteGrid() {
            List<Transform> tempList = new List<Transform>();
            foreach (Transform child in tileParent) {
                tempList.Add(child);
            }
            foreach (var child in tempList) {
                DestroyImmediate(child.gameObject);
            }
            tempList.Clear();
        }
    }
}
