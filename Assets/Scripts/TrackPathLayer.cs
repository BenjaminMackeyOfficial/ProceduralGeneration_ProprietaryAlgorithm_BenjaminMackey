using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackPathLayer : MonoBehaviour
{
    public NoiseMapRefiner refiner;
    public InputActionReference space;


    public void Start()
    {
        //space.action.performed += _ => Regenerate();
    }
    public NoiseMap Regenerate(int x, int y)
    {
        NoiseMap trackMap = new NoiseMap(x, y, new CircleNoise(0.7f, false));
        refiner.RefineMap(trackMap);

        return trackMap;
    }
   
}
