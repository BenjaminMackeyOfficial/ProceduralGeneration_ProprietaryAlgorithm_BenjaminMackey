using UnityEngine;

public class TestBuilder : MonoBehaviour
{
    [SerializeField] NoiseMapMesh noiseMesh;
    [SerializeField] NoiseMapRefiner noiseMapRefiner;

    [SerializeField] float boostVal;
    [SerializeField] Vector2 meshSize;
    private NoiseMap map;
    void Starp()
    {

        map = new NoiseMap(new Vector2(meshSize.x, meshSize.y), new StaticNoise());
        noiseMapRefiner.RefineMap(map);
        //map.IncreaseMapResolution(5);
        noiseMesh.AddMap(map);
    }

}
