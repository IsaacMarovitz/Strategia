using UnityEngine;

public static class PerlinNoise {
    // Generate a Perlin Noise map based off of a seed with a Falloff map
    public static float[,] CalculateNoise(int width, int height, int seed, float scale, float falloffA, float falloffB) {

        float[,] noiseMap = new float[width, height];
        float[,] falloffMap = GenerateFalloffMap(width, height, falloffA,falloffB);

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

    // Generate a Falloff map that makes sure that islands cannot spawn on edges
    public static float[,] GenerateFalloffMap(int width, int height, float falloffA, float falloffB) {
        float[,] map = new float[width, height];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value, falloffA, falloffB);
            }
        }
        return map;
    }

    public static float Evaluate(float value, float falloffA, float falloffB) {
        return Mathf.Pow(value, falloffA) / (Mathf.Pow(value, falloffA) + Mathf.Pow(falloffB - falloffB * value, falloffA));
    }
}