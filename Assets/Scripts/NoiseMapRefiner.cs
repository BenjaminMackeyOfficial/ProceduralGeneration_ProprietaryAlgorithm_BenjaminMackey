using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class RefinerMethod
{
    public abstract void Refine(NoiseMap map, float ammount);
}



public class Smooth : RefinerMethod
{
    public override void Refine(NoiseMap map, float ammount)
    {
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                float addVal = 0;
                float[] adjacent = map.GetPointsAdjacent(i,j);
                for (int k = 0; k < adjacent.Length; k++)
                {
                    addVal +=  adjacent[k];
                }
                if(addVal != 0) addVal = (addVal / adjacent.Length) * ammount;
                map._map[i][j] += addVal;
            }
        }
        
    }
}

public class Perlin : RefinerMethod
{
    public override void Refine(NoiseMap map, float ammount)
    {
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                map._map[i][j] = Mathf.PerlinNoise1D(map._map[i][j]);
            }
        }

    }
}

[System.Serializable]
public class NoiseMapRefiner : MonoBehaviour
{
    [SerializeField] List<RefinerMethod> activeRefiningMethods;
    public float boostVal = 1;

    private void Awake()
    {
        activeRefiningMethods = new List<RefinerMethod>();
        activeRefiningMethods.Add(new Perlin());
        //activeRefiningMethods.Add(new Smooth());
    }

    public void RefineMap(NoiseMap map)
    {
        foreach (RefinerMethod item in activeRefiningMethods)
        {
            item.Refine(map,boostVal);
        }
    }

}
