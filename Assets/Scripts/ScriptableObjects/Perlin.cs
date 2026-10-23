using UnityEngine;

[CreateAssetMenu(fileName = "Perlin", menuName = "Terrain Refining/Perlin")]
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