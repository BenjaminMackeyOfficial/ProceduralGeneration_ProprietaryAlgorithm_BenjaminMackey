using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoiseMap
{
    public int _pixelX { get; private set; }
    public int _pixelY { get; private set; }
    public NoiseGeneration _generator;
    public Vector2 _heighMinAndMaxes {
        get {
            float min = float.MaxValue;
            float max = float.MinValue;
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    if (_map[i, j] < min) min = _map[i, j];
                    if (_map[i, j] > max) max = _map[i, j];
                }
            }
            return new Vector2(min, max);
        }
    }

    public float[,] _map;
    public NoiseMap(Vector2 pixelCount, NoiseGeneration generator)
    {
        _pixelX = (int)pixelCount.x;
        _pixelY = (int)pixelCount.y;

        _generator = generator;
        _map = _generator.GenerateRandomNoise(_pixelX, _pixelY);
    }
    public NoiseMap(int x, int y, NoiseGeneration generator)
    {
        _pixelX = x;
        _pixelY = y;

        _generator = generator;
        _map = _generator.GenerateRandomNoise(_pixelX, _pixelY);
    }

    public void IncreaseMapResolution(int scaleBy)
    {
        int oldX = _pixelX;
        int oldY = _pixelY;

        int newX = (oldX - 1) * scaleBy + 1;
        int newY = (oldY - 1) * scaleBy + 1;
        float[,] newVerts = new float[newX, newY];

        for (int x = 0; x < newX; x++)
        {
            int x0 = Mathf.Min(x / scaleBy, oldX - 2);
            float tx = (x - x0 * scaleBy) / (float)scaleBy;

            for (int y = 0; y < newY; y++)
            {
                int y0 = Mathf.Min(y / scaleBy, oldY - 2);
                float ty = (y - y0 * scaleBy) / (float)scaleBy;

                float bottom = Mathf.Lerp(_map[x0, y0], _map[x0 + 1, y0], tx);
                float top = Mathf.Lerp(_map[x0, y0 + 1], _map[x0 + 1, y0 + 1], tx);

                newVerts[x, y] = Mathf.Lerp(bottom, top, ty);
            }
        }

        _map = newVerts;
        _pixelX = newX;
        _pixelY = newY;
    }

    public float[] GetPointsAdjacent(int x, int y) //returns  all points touching the given point index
    {
        if (_map == null) return new float[0];

        List<float> toReturn = new List<float>();

        if (x < _pixelX - 1) toReturn.Add(_map[x + 1, y]);
        if (x > 0) toReturn.Add(_map[x - 1,y]);

        if (y < _pixelY - 1) toReturn.Add(_map[x,y + 1]);
        if (y > 0) toReturn.Add(_map[x,y - 1]);

        return toReturn.ToArray();
    }
    public static float[] GetPointsAdjacent(float[,] map, int x, int y) //returns  all points touching the given point index
    {
        if (map == null) return new float[0];

        List<float> toReturn = new List<float>();

        if (x < map.GetLength(0) - 1) toReturn.Add(map[x + 1, y]);
        if (x > 0) toReturn.Add(map[x - 1, y]);

        if (y < map.GetLength(1) - 1) toReturn.Add(map[x, y + 1]);
        if (y > 0) toReturn.Add(map[x, y - 1]);

        return toReturn.ToArray();
    }
}

public abstract class NoiseGeneration //these generate noise
{
    public abstract float[,] GenerateRandomNoise(int x, int y);
}

public class StaticNoise : NoiseGeneration
{
    public override float[,] GenerateRandomNoise(int x, int y)
    {
        float[,] returnMap = new float[x, y];

        for (int i = 0; i < x; i++)
        {

            for (int j = 0; j < y; j++)
            {
                returnMap[i, j] = UnityEngine.Random.value;
            }
        }
        return returnMap;
    }
}

public class CircleNoise : NoiseGeneration
{
    private bool _placeRandomly;
    private float _radius;
    public CircleNoise(float scale, bool placeRandomly)
    {
        _placeRandomly = placeRandomly;
        _radius = Mathf.Clamp01(scale); // gets adjusted when generaterandomnoise is called to be actual radius
    }
    public override float[,] GenerateRandomNoise(int x, int y)
    {
        float[,] returnMap = new float[x, y];

        Vector2 centre = new Vector2(x / 2, y / 2);


        
        float _squaredRad = (centre.x * _radius) * (centre.x * _radius);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float dist = (new Vector2(i, j) - centre).sqrMagnitude;
                Debug.Log(dist + " | " + _squaredRad);
                if (dist > _squaredRad) returnMap[i, j] = 0;
                else returnMap[i, j] = 1;
            }
        }
        return returnMap;
    }
}


/*
public class Square : NoiseGeneration
{

    protected override float[,] GenerateRandomNoise(int x, int y)
    {
        return new float[x, y];
    }
}
*/