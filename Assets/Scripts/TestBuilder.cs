using UnityEngine;
using UnityEngine.InputSystem;

public class TestBuilder : MonoBehaviour
{
    [SerializeField] NoiseMapMesh noiseMesh;
    [SerializeField] NoiseMapRefiner noiseMapRefiner;

    [SerializeField] float boostVal;
    [SerializeField] Vector2 meshSize;
    private NoiseMap map;
    private NoiseMap trackMap;
    [SerializeField] InputActionReference space;

    //track stuff
    [SerializeField] TrackPathLayer trackPathLayer;
    //
    void Start()
    {
        space.action.performed += _ => Generate();
    }
    void Generate()
    {
        map = new NoiseMap(new Vector2(meshSize.x, meshSize.y), new StaticNoise());
        noiseMapRefiner.RefineMap(map);


        trackMap = trackPathLayer.Regenerate((int)meshSize.x * 25, (int)meshSize.y * 25);

        map.IncreaseMapResolution(5);

        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                if (trackMap._map[i, j] > 0.5f) map._map[i, j] += trackMap._map[i, j] ;
            }
        }

        
        noiseMesh.AddMap(map);

    }

}
