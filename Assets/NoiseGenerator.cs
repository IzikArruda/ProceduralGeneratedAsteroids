using UnityEngine;
 using System.Collections;
 
 using LibNoise.Unity;
 using LibNoise.Unity.Generator;
 using LibNoise.Unity.Operator;
public class NoiseGenerator : MonoBehaviour {


    public int mapSizeX = 256; // for heightmaps, this would be 2^n +1
    public int mapSizeY = 256; // for heightmaps, this would be 2^n +1

    public float sampleSizeX = 4.0f; // perlin sample size
    public float sampleSizeY = 4.0f; // perlin sample size

    public float sampleOffsetX = 2.0f; // to tile, add size to the offset. eg, next tile across would be 6.0f
    public float sampleOffsetY = 1.0f; // to tile, add size to the offset. eg, next tile up would be 5.0f


    public Renderer cubeRenderer; // renderer texture set for testing

    private Texture2D texture; // texture created for testing

    public Material mat;
    public Texture2D tex;


    public Material asteroidMaterial;

    //  Persistant Functions
    //    ----------------------------------------------------------------------------


    void Start() {

        /* Initilize the material used for the asteroid */
        asteroidMaterial = new Material(Shader.Find("Standard"));

        Generate();
    }


    void Update() {
        //if(Input.GetMouseButtonDown(0))
            //Generate();
    }


    //  Other Functions
    //    ----------------------------------------------------------------------------


    void Generate() {
        Perlin myPerlin = new Perlin();

        ModuleBase myModule = myPerlin;



        // ------------------------------------------------------------------------------------------

        // - Generate -

        // this part generates the heightmap to a texture, 
        // and sets the renderer material texture of a cube to the generated texture


        Noise2D heightMap;

        heightMap = new Noise2D(mapSizeX, mapSizeY, myModule);

        heightMap.GeneratePlanar(
            sampleOffsetX,
            sampleOffsetX + sampleSizeX,
            sampleOffsetY,
            sampleOffsetY + sampleSizeY
            );

        texture = heightMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
        tex = texture;
        cubeRenderer.material.SetTexture("MainTexture", tex);





        /*

        mat.mainTexture = tex;
        cubeRenderer.material = mat;


        cubeRenderer.material.mainTexture = tex;
        cubeRenderer.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = tex.GetPixels();


        
        cubeRenderer.material = asteroidMaterial;
        asteroidMaterial.SetTexture("_MainTex", texture);*/
    }
}
