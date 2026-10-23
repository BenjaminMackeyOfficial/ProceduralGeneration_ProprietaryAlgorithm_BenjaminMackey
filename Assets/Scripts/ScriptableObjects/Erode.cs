using UnityEngine;

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
        if (enableHotspots)
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
                            locationBoost += (devBy != 0 ? 1.5f * Mathf.Pow(1f / devBy, 2f) : 0f);
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
        if (enableConsoleOutputs)
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

