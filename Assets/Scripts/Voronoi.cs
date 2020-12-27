using UnityEngine;
using System.Collections.Generic;

public static class Voronoi {

    public static Texture2D GenerateVoronoi(List<Tile> points) {
        Texture2D returnTexture = new Texture2D(GameManager.Instance.grid.width, GameManager.Instance.grid.height);
        returnTexture.filterMode = FilterMode.Point;
        returnTexture.wrapMode = TextureWrapMode.Repeat;
        List<Color> colors = new List<Color>();
        foreach (var point in points) {
            colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
        }
        for (int x = 0; x < GameManager.Instance.grid.width; x++) {
            for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                returnTexture.SetPixel(x, y, colors[GetClosestPointIndex(new Vector2Int(x, y), points)]);
            }
        }
        returnTexture.Apply();
        return returnTexture;
    }

    public static int GetClosestPointIndex(Vector2Int pixel, List<Tile> points) {
        float smallestDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < points.Count; i++) {
            float distance = ManhattanDistance(pixel, points[i].index);
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