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

        public Transform tileParent;
        public GameObject seaPrefab;
        public GameObject plainsPrefab;
        public GameObject swampPrefab;
        public GameObject mountiansPrefab;
        public GameObject treesPrefab;
        public GameObject cityPrefab;
        public GameObject costalCityPrefab;

        private List<Tile> potentialCityTiles = null;

        void Awake() {
            CreateGrid();
        }

        public void SpawnTiles() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GameObject instantiatedTile;
                    City cityScript;
                    switch (grid[x, y].tileType) {
                        case TileType.Sea:
                            instantiatedTile = GameObject.Instantiate(seaPrefab, new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Plains:
                            instantiatedTile = GameObject.Instantiate(plainsPrefab, new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Swamp:
                            instantiatedTile = GameObject.Instantiate(swampPrefab, new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Mountains:
                            instantiatedTile = GameObject.Instantiate(mountiansPrefab, new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.Trees:
                            instantiatedTile = GameObject.Instantiate(treesPrefab, new Vector3(x * tileWidth, -1, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            break;
                        case TileType.City:
                            instantiatedTile = GameObject.Instantiate(cityPrefab, new Vector3(x * tileWidth, 0, y * tileHeight), Quaternion.Euler(0, 180, 0));
                            instantiatedTile.transform.tag = "City";
                            cityScript = instantiatedTile.GetComponent<City>();
                            cityScript.pos = new Vector2Int(x, y);
                            cityScript.gridScript = this;
                            break;
                        case TileType.CostalCity:
                            instantiatedTile = GameObject.Instantiate(costalCityPrefab, new Vector3(x * tileWidth, 0, y * tileHeight), Quaternion.Euler(0, 180, 0));
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

        public void DeleteGrid() {
            List<Transform> tempList = new List<Transform>();
            foreach (Transform child in tileParent) {
                tempList.Add(child);
            }
            foreach (var child in tempList) {
                DestroyImmediate(child.gameObject);
            }
            potentialCityTiles?.Clear();
            tempList.Clear();
        }

        public bool CostalCheck(int x, int y) {
            if ((x > 0) && (grid[x - 1, y].tileType == TileType.Sea)) {
                return true;
            } else if ((y > 0) && (grid[x, y - 1].tileType == TileType.Sea)) {
                return true;
            } else if ((x < width - 1) && (grid[x + 1, y].tileType == TileType.Sea)) {
                return true;
            } else if ((y < height - 1) && (grid[x, y + 1].tileType == TileType.Sea)) {
                return true;
            } else {
                return false;
            }
        }

        public List<Tile> RadialSearch(Vector2Int pos, int radius) {
            List<Tile> tilesInRange = new List<Tile>();
            Tile startingPoint = grid[pos.x, pos.y];
            foreach (var tile in grid) {
                if ((Mathf.Pow((startingPoint.index.x - tile.index.x), 2) + (Mathf.Pow((startingPoint.index.y - tile.index.y), 2))) < Mathf.Pow(radius, 2)) {
                    tilesInRange.Add(tile);
                }
            }
            return tilesInRange;
        }

        public List<Tile> FloodFill(Tile[,] grid, Tile startingPoint) {
            if (startingPoint.tileType == TileType.Sea) {
                return null;
            }
            List<Tile> islandTiles = new List<Tile>();
            Queue<Tile> tileQueue = new Queue<Tile>();
            islandTiles.Add(startingPoint);
            tileQueue.Enqueue(startingPoint);

            while (tileQueue.Count > 0) {
                Tile n = tileQueue.Peek();
                tileQueue.Dequeue();
                Tile nextTile;

                nextTile = grid[n.index.x - 1, n.index.y];
                if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                    islandTiles.Add(nextTile);
                    tileQueue.Enqueue(nextTile);
                }
                nextTile = grid[n.index.x + 1, n.index.y];
                if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                    islandTiles.Add(nextTile);
                    tileQueue.Enqueue(nextTile);
                }
                nextTile = grid[n.index.x, n.index.y + 1];
                if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                    islandTiles.Add(nextTile);
                    tileQueue.Enqueue(nextTile);
                }
                nextTile = grid[n.index.x - 1, n.index.y - 1];
                if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                    islandTiles.Add(nextTile);
                    tileQueue.Enqueue(nextTile);
                }
            }
            return islandTiles;
        }

        public void CreateGrid() {
            DeleteGrid();
            seed = Random.Range(0, 10000);
            grid = new Tile[width, height];
            potentialCityTiles = new List<Tile>();
            float[,] noiseMap = CalculateNoise(width, height, seed);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    grid[x, y] = new Tile(TileType.Sea, null, new Vector2Int(x, y), null);

                    float sample = noiseMap[x, y];

                    if ((sample >= 0) && (sample < 0.5)) {
                        grid[x, y].tileType = TileType.Sea;
                    } else if ((sample > 0.5) && (sample < 0.6)) {
                        grid[x, y].tileType = TileType.Plains;
                        potentialCityTiles.Add(new Tile(TileType.City, null, new Vector2Int(x, y), null));
                    } else if ((sample > 0.6) && (sample < 0.61)) {
                        grid[x, y].tileType = TileType.Swamp;
                    } else if ((sample > 0.73) && (sample < 0.85)) {
                        grid[x, y].tileType = TileType.Trees;
                    } else if (sample > 0.85) {
                        grid[x, y].tileType = TileType.Mountains;
                    } else {
                        grid[x, y].tileType = TileType.Plains;
                        potentialCityTiles.Add(new Tile(TileType.City, null, new Vector2Int(x, y), null));
                    }
                }
            }
            foreach (var tile in potentialCityTiles) {
                if (CostalCheck(tile.index.x, tile.index.y)) {
                    tile.tileType = TileType.CostalCity;
                }
            }
            CalculateCities();
            SpawnTiles();
        }

        // Perform a Fisher-Yates shuffle on the list to generate cities
        public void CalculateCities() {
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

        public float[,] CalculateNoise(int width, int height, int seed) {

            float[,] noiseMap = new float[width, height];
            float[,] falloffMap = GenerateFalloffMap(width, height);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float xCoord = (float)x / width * scale + seed;
                    float yCoord = (float)y / height * scale + seed;
                    noiseMap[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
            }

            return noiseMap;
        }

        public float[,] GenerateFalloffMap(int width, int height) {
            float[,] map = new float[width, height];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    float x = i / (float)width * 2 - 1;
                    float y = j / (float)height * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    map[i, j] = Evaluate(value);
                }
            }
            return map;
        }

        public float Evaluate(float value) {
            return Mathf.Pow(value, falloffA) / (Mathf.Pow(value, falloffA) + Mathf.Pow(falloffB - falloffB * value, falloffA));
        }
    }
}
