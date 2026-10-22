using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackPathLayer : MonoBehaviour
{
    public NoiseMap terrainMap; //rhe ground the game generates, this is for height (oh i can overlay the track onto this so it flattens the area the track is on)

    public NoiseMap trackMap;
    public NoiseMapMesh mesh;
    public NoiseMapRefiner refiner;
    public Material material;
    public InputActionReference space;

    private List<GameObject> cubes;

    public void Start()
    {
        mesh = new GameObject().AddComponent<NoiseMapMesh>();
        mesh.material = material;
        mesh.size = new Vector3(100, 5, 100);

        space.action.performed += _ => Regenerate();
    }
    private void Regenerate()
    {
        trackMap = new NoiseMap(100, 100, new CircleNoise(0.7f, false));
        refiner.RefineMap(trackMap);

        mesh.AddMap(trackMap);
    }
   
}
