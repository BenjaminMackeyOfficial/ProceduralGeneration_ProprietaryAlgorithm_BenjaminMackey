using System.Collections.Generic;
using UnityEngine;

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

                if (map._map[i, j] > numsAbove) outsideVals.Add((i, j));
            }
        }

        List<(int, int)> lastLayer = new List<(int, int)>(outsideVals);

        for (int layer = 1; layer < thickness; layer++)
        {
            List<(int, int)> nextLayer = new List<(int, int)>();

            foreach (var point in lastLayer)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (Mathf.Abs(dx) + Mathf.Abs(dy) != 1) continue;

                        int x = point.Item1 + dx;
                        int y = point.Item2 + dy;

                        if (x < 0 || x >= map._pixelX || y < 0 || y >= map._pixelY) continue;

                        //if (map._map[x, y] <= numsAbove) continue;
                        if (outsideVals.Contains((x, y))) continue;

                        outsideVals.Add((x, y));
                        nextLayer.Add((x, y));
                    }
                }
            }

            lastLayer = nextLayer;
        }
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if (outsideVals.Contains((i, j))) map._map[i, j] = 1;
                else map._map[i, j] = 0;
            }
        }

    }
}