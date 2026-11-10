using UnityEngine;

[CreateAssetMenu(fileName = "Perlin", menuName = "Terrain Refining/Perlin")]
public class Perlin : RefinerMethod
{
    public override void Refine(NoiseMap map)
    {
        float scramble = CustomRand.Random(-1000f, 1000f);
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                map._map[i, j] = Mathf.PerlinNoise(i + scramble , j + scramble ) + map._map[i,j] * amount;
            }
        }
    }
}