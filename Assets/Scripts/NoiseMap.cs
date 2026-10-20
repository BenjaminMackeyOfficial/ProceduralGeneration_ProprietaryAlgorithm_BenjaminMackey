using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class NoiseMap
{
    public int _pixelX { get; private set; }
    public int _pixelY { get; private set; }

    public float[,] _map;
    public NoiseMap(Vector2 pixelCount)
    {
        _pixelX = (int)pixelCount.x;
        _pixelY = (int)pixelCount.y;

        _map = GenerateRandomNoise(_pixelX, _pixelY);
    }
    
    public void IncreaseMapResolution(int ScaleBy)
    {
        
        _pixelX = _pixelX * ScaleBy;
        _pixelY = _pixelX * ScaleBy;
        float[,] newVerts = new float[_pixelX, _pixelY];

        float lastXcord = 0;
        float lastYcord = 0;

        float nextXcord = 0;
        float nextYcord = 0;
        for (int i = 0; i < _pixelX; i++)
        {
            if(i % ScaleBy == 0) lastXcord = i;
            if(lastXcord + 1 >= _pixelX / ScaleBy) nextXcord = i + scale;
            else nextXcord = -1;

            for (int k = 0; k < _pixelY; k++)
            {
                if(k % ScaleBy == 0) lastYcord = k;
                if(lastYcord + 1 >= _pixelY / ScaleBy) nextYcord = i + scale;
                else nextYcord == -1;
            
                float pointScalar = (
                    (Mathf.Lerp(_map[nextXcord,nextYcord],_map[lastXcord,lastYcord],nextXcord - (i - lastXcord)) *0.5f) +;
                    (Mathf.Lerp(_map[nextXcord,nextYcord],_map[lastXcord,lastYcord],nextXcord - (k - lastXcord)) *0.5f);
                ) 
            }
        }



        

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

    private float[,] GenerateRandomNoise(int x, int y)
    {
        float[,] returnMap = new float[x,y];

        for (int i = 0; i < x; i++)
        {

            for (int j = 0; j < y; j++)
            {
                returnMap[i,j] = Random.value;
            }
        }
        return returnMap;
    }
}
