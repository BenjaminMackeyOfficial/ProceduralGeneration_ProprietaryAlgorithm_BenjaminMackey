using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

[System.Serializable]
public abstract class RefinerMethod : ScriptableObject
{
    [SerializeField] protected float amount = 1;
    public abstract void Refine(NoiseMap map);
}

[CreateAssetMenu(fileName = "Smooth",menuName = "Terrain Refining/Smooth")]
public class Smooth : RefinerMethod
{
    public override void Refine(NoiseMap map)
    {
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                float addVal = 0;
                float[] adjacent = map.GetPointsAdjacent(i, j);

                for (int k = 0; k < adjacent.Length; k++)
                {
                    addVal += adjacent[k];
                }

                if (adjacent.Length > 0)
                {
                    addVal = (addVal / adjacent.Length) * amount;
                }

                map._map[i, j] += addVal;
            }
        }
    }
}

[CreateAssetMenu(fileName = "Perlin",menuName = "Terrain Refining/Perlin")]
public class Perlin : RefinerMethod
{
    public override void Refine(NoiseMap map)
    {
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                map._map[i, j] = Mathf.PerlinNoise1D(map._map[i, j]) * map._map[i, j];
            }
        }
    }
}

[CreateAssetMenu(fileName = "IncreaseResolution", menuName = "Terrain Refining/IncreaseResolution")]
public class IncreaseResolution : RefinerMethod
{
    public override void Refine(NoiseMap map)
    {
        map.IncreaseMapResolution((int)amount);
    }
}

[CreateAssetMenu(fileName = "MountainAndValley",menuName = "Terrain Refining/Mountain And Valley")]
public class MountainAndValley : RefinerMethod
{
    [SerializeField, Range(0f, 1f)] private float _splitNum = 0.5f; //leart about adding constraints to fields today, very cool!

    public override void Refine(NoiseMap map)
    {
        float average = 0f;

        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                average += map._map[i, j];
            }
        }

        average /= map._pixelX * map._pixelY;
        average += average * _splitNum;

        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if (map._map[i, j] > average)
                {
                    map._map[i, j] *= 1f + 0.1f * amount;
                }
                else
                {
                    map._map[i, j] *= 1f - 0.1f * amount;
                }
            }
        }
    }
}

[CreateAssetMenu(fileName = "OverlayMap", menuName = "Terrain Refining/OverlayMap")]
public class OverlayShape : RefinerMethod
{
    [SerializeField] bool cutOut = false; //determines weather the shape is added or subtracted
    [SerializeField] bool Inverse = false;
    
    public override void Refine(NoiseMap map)
    {
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                
            }
        }
    }
}
[CreateAssetMenu(fileName = "Erode", menuName = "Terrain Refining/Erode")]
public class Erode : RefinerMethod
{
    [SerializeField] bool allowIslands = false;
    [SerializeField] float erosionPasses = 100f;
    [SerializeField] float erosionPower = 1f;
    [SerializeField] float erodePointsAbove = 0.5f;
    public override void Refine(NoiseMap map)
    {
        if(!allowIslands)
        {

        }
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if(i == 0 | i >= map._pixelX - 1)
                {
                    map._map[i, j] = 0;
                    continue;
                }
                if (j == 0 | j >= map._pixelY - 1)
                {
                    map._map[i, j] = 0;
                    continue;
                }
                //come back here <--
                if (allTouching(
                    new float[3, 3]
                    {
                        { map._map[i-1, j-1],  map._map[i, j-1],   map._map[i+1, j-1] },
                        { map._map[i-1, j],    0f,                 map._map[i+1, j] },
                        { map._map[i-1, j+1],  map._map[i, j+1],   map._map[i+1, j+1] }
                    }))
                {
                    map._map[i, j] = 1;
                }
                else map._map[i, j] = 0;
            }
        }
    }
    private bool allTouching(float[,] grid)
    {
        //DOESNT NEED TO CHECK MIDDLE CAUSE THE MIDDLE IS ALWAYS GONE

        //checking for straight lines
        if (grid[0, 1] > erodePointsAbove && grid[2, 1] > erodePointsAbove) return false; //checking for vert line
        if (grid[1, 0] > erodePointsAbove && grid[1, 2] > erodePointsAbove) return false; //checking for hori line
        if (grid[0, 0] > erodePointsAbove && grid[2, 2] > erodePointsAbove) return false; //checking for diag line
              

        //checking for isolated corners
        if (grid[1, 0] > erodePointsAbove && grid[0, 1] > erodePointsAbove) return false;//top left
        if (grid[1, 0] > erodePointsAbove && grid[2, 1] > erodePointsAbove) return false;//top right

        if (grid[1, 2] > erodePointsAbove && grid[0, 1] > erodePointsAbove) return false;//bottom left
        if (grid[1, 2] > erodePointsAbove && grid[2, 1] > erodePointsAbove) return false;//bottom right

        return true;
    }
}


[CreateAssetMenu(fileName = "Bowlify",menuName = "Terrain Refining/Bowlify")]
public class Bowlify : RefinerMethod
{
    private float MathyCalc(float num)
    {
        return -Mathf.Cos(num * (Mathf.PI / 2f)) + 1f;
    }

    public override void Refine(NoiseMap map)
    {
        float midX = map._pixelX * 0.5f;
        float midY = map._pixelY * 0.5f;

        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                float multVal = Mathf.Clamp(
                    MathyCalc(Mathf.Abs(i - midX) / midX) +
                    MathyCalc(Mathf.Abs(j - midY) / midY),
                    0f, 1f
                ) * amount;

                map._map[i, j] += multVal;
            }
        }
    }
}


public class NoiseMapRefiner : MonoBehaviour
{
    [SerializeField]
    private List<RefinerMethod> activeRefiningMethods = new();

    public void RefineMap(NoiseMap map)
    {
        foreach (RefinerMethod item in activeRefiningMethods)
        {
            if (item == null) continue;

            item.Refine(map);
        }
    }
}
