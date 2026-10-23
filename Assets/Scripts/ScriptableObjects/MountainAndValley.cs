using UnityEngine;

[CreateAssetMenu(fileName = "MountainAndValley", menuName = "Terrain Refining/Mountain And Valley")]
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