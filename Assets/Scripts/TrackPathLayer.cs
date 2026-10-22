using UnityEngine;

public class TrackPathLayer : MonoBehaviour
{
    public NoiseMap terrainMap; //rhe ground the game generates, this is for height (oh i can overlay the track onto this so it flattens the area the track is on)

    public NoiseMap trackMap;
    public NoiseMapMesh mesh;

    public void Start()
    {
        //trackMap = new NoiseMap(terrainMap._pixelX, terrainMap._pixelY, new CircleNoise(0.7f, false));
        trackMap = new NoiseMap(100, 100, new CircleNoise(0.7f, false));
        mesh = new GameObject().AddComponent<NoiseMapMesh>();

        mesh.size = new Vector3(100, 5, 100);
        mesh.AddMap(trackMap);
    }
}
