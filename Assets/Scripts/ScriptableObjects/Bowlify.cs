using UnityEngine;

[CreateAssetMenu(fileName = "Bowlify", menuName = "Terrain Refining/Bowlify")]
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