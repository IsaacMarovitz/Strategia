using UnityEngine;
using System.Collections.Generic;

public static class Voronoi {

    public static Texture2D GenerateVoronoi(List<City> cities) {
        Texture2D returnTexture = new Texture2D(GameManager.Instance.grid.width, GameManager.Instance.grid.height);
        returnTexture.filterMode = FilterMode.Point;
        returnTexture.wrapMode = TextureWrapMode.Repeat;
        List<Color> colors = new List<Color>();
        foreach (var point in cities) {
            colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
        }
        for (int x = 0; x < GameManager.Instance.grid.width; x++) {
            for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                returnTexture.SetPixel(x, y, colors[GetClosestPointIndex(new Vector2Int(x, y), cities)]);
            }
        }
        returnTexture.Apply();
        return returnTexture;
    }

    public static City GetCityOfInfluence(Vector2Int pos, List<City> cities) {
        float smallestDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < cities.Count; i++) {
            float distance = ManhattanDistance(pos, cities[i].pos);
            if (distance < smallestDistance) {
                smallestDistance = distance;
                index = i;
            }
        }
        return cities[index];
    }

    public static int GetClosestPointIndex(Vector2Int pixel, List<City> cities) {
        float smallestDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < cities.Count; i++) {
            float distance = ManhattanDistance(pixel, cities[i].pos);
            if (distance < smallestDistance) {
                smallestDistance = distance;
                index = i;
            }
        }
        return index;
    }

    public static float ManhattanDistance(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}