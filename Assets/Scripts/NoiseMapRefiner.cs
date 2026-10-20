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
                map._map[i,j] += addVal;
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
                map._map[i,j] = Mathf.PerlinNoise1D(map._map[i,j]) *  map._map[i,j];
            }
        }
    }
}

public class MountainAndValley : RefinerMethod
{
    private float _splitNum = 0.5f; //the number that determines what gets bosted

    private float average = 0;
    
    private float lowerAvg = 0;
    private float upperAvg = 0;

    public MountainAndValley(float splitNum)
    {
        _splitNum = splitNum;
    }

    public override void Refine(NoiseMap map, float ammount)
    {
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                average += map._map[i,j];
            }
        }
        
        average /= map._pixelX * map._pixelY;
        average = average +  average * _splitNum; //this is the number everythings gotta abide by
        Debug.Log(average);

        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if(map._map[i,j] > average) map._map[i,j] *= 1.1f;
                else map._map[i,j] *= 0.1f;
            }
        }
    }
}

public class Bowlify : RefinerMethod
{
    float midX = 0;
    float midY = 0;
    private float mathyCalc(float num)
    {
        float toReturn = -Mathf.Cos(num * (Mathf.PI /2) ) + 1;
        return toReturn;
    }
    public override void Refine(NoiseMap map, float ammount)
    {
        midX = map._pixelX * 0.5f;
        midY = map._pixelY * 0.5f;
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                float multVal = Mathf.Clamp(
                    mathyCalc(Mathf.Abs((float)i - midX) / midX) +
                    mathyCalc(Mathf.Abs((float)j - midY) / midY),
                    0,1
                )* ammount;
                //Debug.Log(Mathf.Abs(((float)i - midX) / map._pixelX) + " | " +  (Mathf.Abs((float)j - midY) / map._pixelY));
                map._map[i,j] += multVal;
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
        activeRefiningMethods.Add(new MountainAndValley(0.6f));
        activeRefiningMethods.Add(new Bowlify());
        //activeRefiningMethods.Add(new MountainAndValley(0.2f));
        
        
    }

    public void RefineMap(NoiseMap map)
    {
        foreach (RefinerMethod item in activeRefiningMethods)
        {
            item.Refine(map,boostVal);
        }
    }

}
