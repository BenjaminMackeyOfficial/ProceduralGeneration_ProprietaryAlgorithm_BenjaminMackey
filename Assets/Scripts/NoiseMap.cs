using System.Collections.Generic;
using NUnit.Framework;
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

    public float[] GetPointsAdjacent(int x, int y) //returns  all points touching the given point index
    {
        if (_map == null) return new float[0];

        List<float> toReturn = new List<float>();

        if (x < _pixelX - 1) toReturn.Add(_map[x + 1][ y]);
        if (x > 0) toReturn.Add(_map[x - 1][y]);

        if (y < _pixelY - 1) toReturn.Add(_map[x][y + 1]);
        if (y > 0) toReturn.Add(_map[x][y - 1]);

        return toReturn.ToArray();
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
