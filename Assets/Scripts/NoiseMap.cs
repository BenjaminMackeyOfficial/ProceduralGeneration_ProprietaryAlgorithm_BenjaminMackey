using UnityEngine;

public class NoiseMap
{
    public int _pixelX { get; private set; }
    public int _pixelY { get; private set; }

    public float[][] _map;
    public NoiseMap(Vector2 pixelCount)
    {
        _pixelX = (int)pixelCount.x;
        _pixelY = (int)pixelCount.y;

        _map = GenerateRandomNoise(_pixelX, _pixelY);
    }

    private float[][] GenerateRandomNoise(int x, int y)
    {
        float[][] returnMap = new float[x][];

        for (int i = 0; i < x; i++)
        {
            returnMap[i] = new float[y];

            for (int j = 0; j < y; j++)
            {
                returnMap[i][j] = Random.value;
            }
        }
        return returnMap;
    }
}
