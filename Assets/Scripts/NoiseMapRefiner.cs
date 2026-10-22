using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using static UnityEditor.Progress;

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

[CreateAssetMenu(fileName = "Outline", menuName = "Terrain Refining/Outline")]
public class Outline : RefinerMethod
{
    [SerializeField] int thickness = 1;
    [SerializeField] float numsAbove = 0.5f;

    public override void Refine(NoiseMap map)
    {
        List<(int, int)> outsideVals = new List<(int, int)>();
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if (map._map[i, j] < numsAbove)
                {
                    continue;
                }

                //checking, the edges of the map always get dissolved
                if (i == 0 | i >= map._pixelX - 1)
                {
                    outsideVals.Add((i, j));
                    continue;
                }
                if (j == 0 | j >= map._pixelY - 1)
                {
                    outsideVals.Add((i, j));
                    continue;
                }

                //seeing if it can see white
                if (map.GetPointsAdjacentAbove(i, j, numsAbove).Length >= 4)
                {
                    continue;
                }

                if (map._map[i,j] > numsAbove) outsideVals.Add((i, j));
            }
        }
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if(outsideVals.Contains((i,j))) map._map[i,j] = 1;
                else map._map[i,j] = 0;
            }
        }

    }
}

[CreateAssetMenu(fileName = "Erode", menuName = "Terrain Refining/Erode")]
public class Erode : RefinerMethod
{
    
    [SerializeField] int erosionSeed = 1;
    [SerializeField] bool allowIslands = false;
    [SerializeField] float erosionPasses = 100f;
    [SerializeField, Range(0f, 10f)] float erosionPower = 0.1f;
    [SerializeField] float erodePointsAbove = 0.5f;

    [Header("Extra fun options")]
    [SerializeField] bool enableHotspots = false;
    [SerializeField] Vector2[] disolveHotspots; //makes it so points around the hotspot are more likeley to dissolve... 0,0 is the middle
    [SerializeField] int numberOfRandomHotspots;
    [Header("Debugging")]
    [SerializeField] bool enableConsoleOutputs; 

    public override void Refine(NoiseMap map)
    {
        
        int dissolved = 0;
        float skippedCauseNotEnoughAdjPoints = 0;
        float tooLow = 0;
        float failedCornerCheck = 0;



        Vector2[] localHotspots = new Vector2[disolveHotspots.Length + numberOfRandomHotspots];
        if(enableHotspots)
        {
            disolveHotspots.CopyTo(localHotspots, 0);
            for (int i = 0; i < disolveHotspots.Length; i++)
            {
                localHotspots[i] += new Vector2(map._pixelX / 2, map._pixelY / 2);
            }
            for (int i = 0; i < localHotspots.Length; i++)
            {
                if (localHotspots[i] != null) continue;
                localHotspots[i] = new Vector2(
                    CustomRand.Random(0, map._pixelX, erosionSeed),
                    CustomRand.Random(0, map._pixelY, erosionSeed)
                    );
            }
        }

        float disolveChance = 1f;
        float locationBoost = 1f;
        int NumOfAdjPoints = 0;

        for (int r = 0; r < erosionPasses; r++)
        {
            for (int i = 0; i < map._pixelX; i++)
            {
                for (int j = 0; j < map._pixelY; j++)
                {
                    if (map._map[i, j] < erodePointsAbove)
                    {
                        tooLow++;
                        continue;
                    }

                    //checking, the edges of the map always get dissolved
                    if (i == 0 | i >= map._pixelX - 1)
                    {
                        map._map[i, j] = 0;
                        continue;
                    }
                    if (j == 0 | j >= map._pixelY - 1)
                    {
                        map._map[i, j] = 0;
                        continue;
                    }

                   
                    //seeing if it can see white
                    NumOfAdjPoints = map.GetPointsAdjacentAbove(i, j, erodePointsAbove).Length;
                    if (NumOfAdjPoints >= 4)
                    {
                        skippedCauseNotEnoughAdjPoints++;
                        continue;
                    }
                    //checking to see if disolving the middle would open white space
                    if (!allowIslands && !allTouching(
                        new float[3, 3]
                        {
                            { map._map[i-1, j-1],  map._map[i, j-1],   map._map[i+1, j-1] },
                            { map._map[i-1, j],    0f,                 map._map[i+1, j] },
                            { map._map[i-1, j+1],  map._map[i, j+1],   map._map[i+1, j+1] }
                        }))
                    {
                        failedCornerCheck++;
                        continue;
                    }

                    //the actual weighing for disolving

                    disolveChance = 1f;
                    locationBoost = 0.5f;

                    if (enableHotspots)
                    {
                        foreach (Vector2 item in localHotspots)
                        {
                            float devBy = Mathf.Abs(i - item.x) + Mathf.Abs(j - item.y);
                            locationBoost += (devBy != 0 ? 1.5f * Mathf.Pow(1f / devBy, 4f) : 0f);
                        }
                    }

                    if (erosionSeed == 0) disolveChance -= CustomRand.Random01() * (1 + erosionPower) * locationBoost;
                    else disolveChance -= CustomRand.Random01(erosionSeed) * (1 + erosionPower) * locationBoost;



                    if (disolveChance < 0)
                    {
                        map._map[i, j] = 0; //dissolves
                        dissolved++;
                    }

                }
            }
        }
        if(enableConsoleOutputs)
        {
            Debug.Log("Failed Corner Check: " + failedCornerCheck);
            Debug.Log("Too Low: " + tooLow);
            Debug.Log("Not enough adjacent: " + skippedCauseNotEnoughAdjPoints);
            Debug.Log("Dissolved: " + dissolved);
        }
    }
    private bool allTouching(float[,] grid) //this should be done with some kind of masking but, oh well
    {
        // THE MIDDLE IS ALWAYS GONE SO IT ASSUMES

        //straight lines
        if (grid[0, 1] > erodePointsAbove && grid[2, 1] > erodePointsAbove) return false;
        if (grid[1, 0] > erodePointsAbove && grid[1, 2] > erodePointsAbove) return false;
        if (grid[0, 0] > erodePointsAbove && grid[2, 2] > erodePointsAbove) return false;
        if (grid[2, 0] > erodePointsAbove && grid[0, 2] > erodePointsAbove) return false;

        
        //corners
        if (grid[0, 0] > erodePointsAbove && grid[0, 1] <= erodePointsAbove && grid[1, 0] <= erodePointsAbove) return false; // Top left
        if (grid[0, 2] > erodePointsAbove && grid[0, 1] <= erodePointsAbove && grid[1, 2] <= erodePointsAbove) return false; // Top right
        if (grid[2, 0] > erodePointsAbove && grid[1, 0] <= erodePointsAbove && grid[2, 1] <= erodePointsAbove) return false; // Bottom left
        if (grid[2, 2] > erodePointsAbove && grid[1, 2] <= erodePointsAbove && grid[2, 1] <= erodePointsAbove) return false; // Bottom right

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
