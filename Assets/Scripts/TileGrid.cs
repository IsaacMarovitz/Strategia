using System.Collections.Generic;
using UnityEngine;

namespace Strategia {
    public class TileGrid : MonoBehaviour {

        public int width = 100;
        public int height = 100;
        public float tileWidth = 2;
        public float tileHeight = 2;
        public float falloffA = 6f;
        public float falloffB = 5f;
        public float scale = 8f;
        public int numberOfCostalCities = 10;
        public int numberOfCities = 10;
        public Tile[,] grid;
        public List<City> cityTiles;
        [Range(0, 10000)]
        public int seed;
        public int islandCount;
        public int minimumIslandArea;
        public Transform tileParent;
        // Sea, Plains, Swamp, Mountains, Trees, City, Costal City
        public GameObject[] prefabs = new GameObject[7];
        [Space(10)]
        public Texture2D voronoiTexture;
        public Tile foundTile;

        public int MaxSize {
            get {
                return width * height;
            }
        }

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
                    grid[x, y] = new Tile(TileType.Sea, null, new Vector2Int(x, y), 0);
                    grid[x, y].walkable = true;

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
            foreach (var tile in grid) {
                tile.cityOfInfluence = Voronoi.GetCityOfInfluence(tile.pos, cityTiles);
            }
            voronoiTexture = Voronoi.GenerateVoronoi(cityTiles, this);
        }

        // Assign an island index to every tile and destroy islands that are too small
        public void CalculateIslands() {
            int islandIndex = 1;
            foreach (var tile in grid) {
                if (tile.islandIndex == 0 && tile.tileType != TileType.Sea) {
                    List<Tile> islandTiles = GridUtilities.FloodFill(tile, grid);
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
                    if (GridUtilities.CostalCheck(tile.pos, this)) {
                        potentialCityTiles.Add(new Tile(TileType.CostalCity, null, tile.pos, tile.islandIndex));
                    } else {
                        potentialCityTiles.Add(new Tile(TileType.City, null, tile.pos, tile.islandIndex));
                    }
                }
            }
            potentialCityTiles = FisherYates(potentialCityTiles);
            int calculatedCostalCities = 0;
            int calculatedCities = 0;
            bool outOfCities = false;

            while (!outOfCities) {
                foreach (var city in potentialCityTiles) {
                    if ((city.tileType == TileType.CostalCity) && (calculatedCostalCities < numberOfCostalCities)) {
                        calculatedCostalCities++;
                        grid[city.pos.x, city.pos.y].tileType = TileType.CostalCity;
                    } else if ((city.tileType == TileType.City) && (calculatedCities < numberOfCities)) {
                        calculatedCities++;
                        grid[city.pos.x, city.pos.y].tileType = TileType.City;
                    }
                }
                outOfCities = true;
            }
            potentialCityTiles.Clear();
        }

        public List<T> FisherYates<T>(List<T> type) {
            List<T> returnList = type;
            int n = returnList.Count;
            while (n > 1) {
                n--;
                int k = Random.Range(0, n + 1);
                var value = returnList[k];
                returnList[k] = returnList[n];
                returnList[n] = value;
            }
            return returnList;
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
                            cityTiles.Add(cityScript);
                            break;
                        case TileType.CostalCity:
                            instantiatedTile = GameObject.Instantiate(prefabs[6], new Vector3(x * tileWidth, 0, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            instantiatedTile.transform.tag = "City";
                            cityScript = instantiatedTile.GetComponent<City>();
                            cityScript.pos = new Vector2Int(x, y);
                            cityTiles.Add(cityScript);
                            break;
                        default:
                            instantiatedTile = null;
                            break;
                    }
                    if (instantiatedTile != null) {
                        instantiatedTile.transform.parent = tileParent;
                        instantiatedTile.name = x + ", " + y;
                        TileTag tileTag = instantiatedTile.GetComponent<TileTag>();
                        tileTag.pos = new Vector2Int(x, y);
                        grid[x, y].gameObject = instantiatedTile;
                    }
                }
            }
        }

        public City ChoosePlayerCity() {
            foreach (var city in cityTiles) {
                if (grid[city.pos.x, city.pos.y].tileType == TileType.CostalCity) {
                    City cityScript = city.gameObject.GetComponent<City>();
                    if (!cityScript.isOwned) {
                        Debug.Log("<b>Tile Grid:</b> Player starting city chosen");
                        return cityScript;
                    }
                }
            }
            throw new System.Exception("No starting city found!");
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
            cityTiles?.Clear();
        }
#if UNITY_EDITOR
        public void FindTile(Vector2Int pos) {
            foundTile = grid[pos.x, pos.y];
        }
#endif
    }
}
