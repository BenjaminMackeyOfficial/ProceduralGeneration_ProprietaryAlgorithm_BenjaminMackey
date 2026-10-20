using UnityEngine;

public class NoiseMapMesh : MonoBehaviour
{
    [Header("Adjustables")]
    public float meshHeighMulti = 1; //determines the height scale of the map
    public float textureScale = 1;
    public float meshScale = 1;
    public Material material;

    [Header("non adjustables")]
    public NoiseMap map { get; private set; }

    public Mesh mesh { get; private set; }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public Vector3[] meshVertecies;
    public Vector2[] meshUV;
    public int[] meshTriangles;

    public void AddMap(NoiseMap handedMap)
    {
        map = handedMap;
        ReGenerateMesh();
    }

    private void ReGenerateVertecies()
    {

    }

    private void ReGenerateMesh()
    {
        if (map == null) return;

        //Placing vertecies----------------+triangles------
        meshVertecies = GenerateVertecies(out meshTriangles, out meshUV);
        //-------------------------------------------------

        //AddingUvMaps-----------------------------------------
        //?
        //---------------------------------------------------------
        mesh = new Mesh();

        mesh.vertices = meshVertecies;
        mesh.triangles = meshTriangles;
        mesh.uv = meshUV;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material = material;
    }

    private Vector3[] GenerateVertecies()
    {
        Vector3[] generatin = new Vector3[map._pixelX * map._pixelY];
        int count = 0;
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                generatin[count] = new Vector3(i, map._map[i,j], j);
                count++;
            }
        }
        return generatin;
    }
    private Vector3[] GenerateVertecies(out int[] triangles, out Vector2[] UVs)
    {
        Vector3[] generatin = new Vector3[map._pixelX * map._pixelY];

        triangles = new int[((map._pixelX - 1) * (map._pixelY - 1) * 2) * 3];

        UVs = new Vector2[generatin.Length];

        int count = -1;
        int triangleCount = 0;
        for (int i = 0; i < map._pixelX; i++)
        {
            for (int j = 0; j < map._pixelY; j++)
            {
                count++;
                generatin[count] = new Vector3(i, map._map[i,j] * meshHeighMulti, j) * meshScale;


                //UV mapping---------------------------------------
                UVs[count] = new Vector2((float)i / (float)map._pixelX, (float)j / (float)map._pixelY) * textureScale; //thought this would be harder lol
                //----------------------------------------------


                //triangle stuff--------------
                if (i == 0) continue;//skips over the first layer cause it goes down 
                if (j >= map._pixelY - 1) continue;//skips the far right

                int vert1, vert2, vert3, vert4; //all the corners. (all the deer.. yeah all the deer cmon more deer)
                vert1 = count - map._pixelY; //top left
                vert2 = count - (map._pixelY - 1); //top right
                vert3 = count + 1; //bottom right
                vert4 = count; //bottom left
                //Debug.Log(i + " | " + j + " | TopL = " + vert1 + " TopR = " + vert2 + " BotL = " + vert4 + " BotR = " +vert3);
                //top half of triangle
                triangles[triangleCount] = vert1;
                triangles[triangleCount + 1] = vert2;
                triangles[triangleCount + 2] = vert4;
                triangleCount += 3;
                //bottom half
                triangles[triangleCount] = vert2;
                triangles[triangleCount + 1] = vert3;
                triangles[triangleCount + 2] = vert4;
                triangleCount += 3;
                //

                //----------------------------
            }
        }
        return generatin;
    }


    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        
    }
}
