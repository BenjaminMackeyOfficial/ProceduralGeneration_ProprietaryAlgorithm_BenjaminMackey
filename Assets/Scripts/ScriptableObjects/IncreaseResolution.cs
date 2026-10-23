
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseResolution", menuName = "Terrain Refining/IncreaseResolution")]
public class IncreaseResolution : RefinerMethod
{
    public override void Refine(NoiseMap map)
    {
        map.IncreaseMapResolution((int)amount);
    }
}