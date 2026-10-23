using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CircleDissolve", menuName = "Terrain Refining/CircleDissolve")]
public class CircleDissolve : RefinerMethod
{
    [SerializeField, Min(1)] int radius = 10;
    [SerializeField, Min(1)] int circlesToRemove = 1;
    [SerializeField, Min(1)] int attemptsPerCircle = 30;
    [SerializeField] float numsAbove = 0.5f;

    private static readonly int[] disX = { -1, 1, 0, 0 };
    private static readonly int[] disY = { 0, 0, -1, 1 };

    public override void Refine(NoiseMap map)
    {
        int[,] islands = LabelIslands(map, out int compCount);

        for (int circle = 0; circle < circlesToRemove; circle++)
        {
            bool accepted = false;

           
            List<Vector2> centers = GetShapePoints(map); //possible centres

            if (centers.Count == 0) break;

            for (int attempt = 0; attempt < attemptsPerCircle; attempt++)
            {
                
                Vector2 center = centers[Random.Range(0, centers.Count)]; //random point on shape

                bool[,] circleMask = new bool[map._pixelX, map._pixelY];

                List<Vector2> circleCells = MakeCircleMask(map, center, circleMask);

                if (!WillBeConnected(map, islands, compCount, circleMask)) continue;//checking if it splits the shape
                if (!CanReachOutside(map, circleMask, circleCells))continue;//checking if holle connects to outisde

                // Both checks passed: remove the circle.
                foreach (Vector2 point in circleCells)
                {
                    if (map._map[(int)point.x, (int)point.y] > numsAbove) map._map[(int)point.x, (int)point.y] = 0;
                }

                accepted = true;
                break;
            }

            // couldnt find circle
            if (!accepted) break;
        }
    }

    private List<Vector2> GetShapePoints(NoiseMap map)
    {
        List<Vector2> points = new List<Vector2>();

        for (int x = 0; x < map._pixelX; x++)
        {
            for (int y = 0; y < map._pixelY; y++)
            {
                if (map._map[x, y] > numsAbove) points.Add(new Vector2(x, y));
            }
        }

        return points;
    }

    private List<Vector2> MakeCircleMask(NoiseMap map,Vector2 center,bool[,] circleMask)
    {
        List<Vector2> cells = new List<Vector2>();
        int radiusSquared = radius * radius;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (i * i + j * j > radiusSquared) continue;

                int x = (int)center.x + i;
                int y = (int)center.y + j;

                if (x < 0 || x >= map._pixelX || y < 0 || y >= map._pixelY) continue;

                circleMask[x, y] = true;
                cells.Add(new Vector2(x, y));
            }
        }

        return cells;
    }

    private int[,] LabelIslands(NoiseMap map, out int compCount) //gets each indivisual island, not really needed but safe include
    {
        int[,] labels = new int[map._pixelX, map._pixelY];
        compCount = 0;

        Queue<Vector2> queue = new Queue<Vector2>(); //learned about queues today, very cool!

        for (int x = 0; x < map._pixelX; x++)
        {
            for (int y = 0; y < map._pixelY; y++)
            {
                if (labels[x, y] != 0 || map._map[x, y] <= numsAbove) continue;

                compCount++;
                labels[x, y] = compCount;
                queue.Enqueue(new Vector2(x, y));

                while (queue.Count > 0)
                {
                    Vector2 point = queue.Dequeue();
                    /*
                    for (int d = 0; d < 4; d++)
                    {
                        
                    }
                    */
                    for (int i = 0; i < 4; i++)
                    {
                        int NewX = (int)point.x + disX[i];
                        int NewY = (int)point.y + disY[i];

                        if (NewX < 0 || NewX >= map._pixelX || NewY < 0 || NewY >= map._pixelY) continue;

                        if (labels[NewX, NewY] != 0 || map._map[NewX, NewY] <= numsAbove) continue;

                        labels[NewX, NewY] = compCount;
                        queue.Enqueue(new Vector2(NewX, NewY));
                    }
                }
            }
        }

        return labels;
    }

    private bool WillBeConnected( NoiseMap map, int[,] originalLabels, int compCount, bool[,] circleMask)
    {
        int[] remainingPixels = new int[compCount + 1];
        int[] connectedPieces = new int[compCount + 1];
        bool[,] visited = new bool[map._pixelX, map._pixelY];

        Queue<Vector2> queue = new Queue<Vector2>();

        for (int x = 0; x < map._pixelX; x++)
        {
            for (int y = 0; y < map._pixelY; y++)
            {
                int id = originalLabels[x, y];

                if (id == 0 || circleMask[x, y] || map._map[x, y] <= numsAbove) continue;

                remainingPixels[id]++;

                if (visited[x, y]) continue;

                connectedPieces[id]++;

                // shape would be split into multiple piecec
                if (connectedPieces[id] > 1) return false;

                visited[x, y] = true;
                queue.Enqueue(new Vector2(x, y));

                while (queue.Count > 0)
                {
                    Vector2 point = queue.Dequeue();

                    for (int i = 0; i < 4; i++)
                    {
                        int newX = (int)point.x + disX[i];
                        int newY = (int)point.y + disY[i];

                        if (newX < 0 || newX >= map._pixelX || newX < 0 || newX >= map._pixelY) continue;

                        if (visited[newX, newY] ||
                            originalLabels[newX, newY] != id ||
                            circleMask[newX, newY] ||
                            map._map[newX, newY] <= numsAbove)
                        {
                            continue;
                        }    
                            
                        visited[newX, newY] = true;
                        queue.Enqueue(new Vector2(newX, newY));
                    }
                }
            }
        }

        // make sre dont remoe original comp
        for (int id = 1; id <= compCount; id++)
        {
            if (remainingPixels[id] == 0 ||
                connectedPieces[id] != 1)
                return false;
        }

        return true;
    }

    private bool CanReachOutside(NoiseMap map,bool[,] circleMask,List<Vector2> circleCells)
    {
        bool[,] reachable = new bool[map._pixelX, map._pixelY];
        Queue<Vector2> queue = new Queue<Vector2>();


        for (int x = 0; x < map._pixelX; x++)
        {
            AddBackgroundSeed(map, circleMask, reachable, queue, x, 0);
            AddBackgroundSeed(map, circleMask, reachable, queue, x, map._pixelY - 1);
        }

        for (int y = 0; y < map._pixelY; y++)
        {
            AddBackgroundSeed(map, circleMask, reachable, queue, 0, y);
            AddBackgroundSeed(map, circleMask, reachable, queue, map._pixelX - 1, y);
        }

        while (queue.Count > 0)
        {
            Vector2 point = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = (int)point.x + disX[i];
                int ny = (int)point.y + disY[i];

                if (nx < 0 || nx >= map._pixelX || ny < 0 || ny >= map._pixelY) continue;

                if (reachable[nx, ny])continue;

                if (!circleMask[nx, ny] && map._map[nx, ny] > numsAbove) continue;

                reachable[nx, ny] = true;
                queue.Enqueue(new Vector2(nx, ny));
            }
        }

        foreach (Vector2 point in circleCells)
        {
            if (map._map[(int)point.x, (int)point.y] > numsAbove &&!reachable[(int)point.x, (int)point.y])return false;
        }

        return true;
    }

    private void AddBackgroundSeed( NoiseMap map, bool[,] circleMask, bool[,] reachable, Queue<Vector2> queue, int x, int y)
    {
        if (reachable[x, y])return;

        if (!circleMask[x, y] && map._map[x, y] > numsAbove) return;

        reachable[x, y] = true;
        queue.Enqueue(new Vector2(x, y));
    }
}