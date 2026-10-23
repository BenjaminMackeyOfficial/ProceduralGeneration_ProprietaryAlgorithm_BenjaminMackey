
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class RefinerMethod : ScriptableObject
{
    [SerializeField] protected float amount = 1;
    public abstract void Refine(NoiseMap map);
}
/*
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
*/







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
