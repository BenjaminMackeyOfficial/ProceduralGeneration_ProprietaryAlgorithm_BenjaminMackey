using UnityEngine;

public class TestBuilder : MonoBehaviour
{
    [SerializeField] NoiseMapMesh noiseMesh;
    [SerializeField] NoiseMapRefiner noiseMapRefiner;

    [SerializeField] float boostVal;
    [SerializeField] Vector2 meshSize;
    private NoiseMap map;
    void Start()
    {
        noiseMapRefiner.boostVal = boostVal;
        map = new NoiseMap(new Vector2(meshSize.x, meshSize.y));
        noiseMapRefiner.RefineMap(map);
        noiseMesh.AddMap(map);
    }
    private void GenMesh()
    {
        map = new NoiseMap(new Vector2(200, 200));
        noiseMapRefiner.RefineMap(map);
        noiseMesh.AddMap(map);
    }

}
