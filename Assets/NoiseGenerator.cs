using UnityEngine;
 using System.Collections;
using System.IO;

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

    public Texture2D texture; // texture created for testing
    
    public Material asteroidMaterial;

    //  Persistant Functions
    //    ----------------------------------------------------------------------------


    void Start() {


        /* Create the noise mesh */
        //Generate();

        /* Assign the material to the asteroid */
        //GetComponent<MeshRenderer>().material = asteroidMaterial;


        //TestGenerate();



        //Encode the texture to a png
        //byte[] bytes = texture.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

        //Load the texture
        //byte[] readBytes = File.ReadAllBytes(Application.dataPath + "/../SavedScreen.png");
        //texture.LoadImage(bytes);


        //create a new material for the plane
        //Material newMaterial = new Material(Shader.Find("Diffuse"));
        //newMaterial.mainTexture = texture;
        //GetComponent<MeshRenderer>().material = newMaterial;
    }

    void TestGenerate() {
        /*
         * Generate a noise map and save it to a file to test it
         */
        int mapX = 512;
        int mapY = 256;     
        float south = -90.0f;
        float north = 90.0f;
        float west = -180.0f;
        float east = 180.0f;
        Perlin myPerlin = new Perlin();
        ModuleBase myModule = myPerlin;
        Noise2D heightMap;

        heightMap = new Noise2D(mapX, mapY, myModule);
        heightMap.GenerateSpherical(south, north, west, east);

        /* Encode the texture to a png */
        Texture2D heightMapTexture = heightMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
        SaveTexture(heightMapTexture);
    }


    void SaveTexture(Texture2D picture) {
        /*
         * Save the given texture into the project folder
         */
        byte[] bytes = picture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        Debug.Log("SAVED PICTURE");
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


        //Debug.Log(cubeRenderer.material.mainTexture.name);
        texture = heightMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);

        cubeRenderer.material.mainTexture = texture;
        //cubeRenderer.material.SetTexture("MainTexture", texture);
        texture.name = "noise";

        asteroidMaterial.mainTexture = texture;
        GetComponent<Renderer>().material.mainTexture = texture;
        GetComponent<Renderer>().material.SetTexture("_MainTexture", texture);
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





    public byte[] CreateSphericalPerlinTexture() {
        /*
         * Generate a spherical texture2D of perlin noise
         */
        int mapX = 512;
        int mapY = 256;
        float south = -90.0f;
        float north = 90.0f;
        float west = -180.0f;
        float east = 180.0f;
        Perlin myPerlin = new Perlin();
        ModuleBase myModule = myPerlin;
        Noise2D heightMap;

        heightMap = new Noise2D(mapX, mapY, myModule);
        heightMap.GenerateSpherical(south, north, west, east);
        
        texture = heightMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);


        byte[] bytes = texture.EncodeToPNG();

        return bytes;
    }

    public void CreateSphericalPerlinTexture(ref byte[] height, ref byte[] normal) {
        /*
         * Generate a spherical mapping of a height and normal map
         */
        int mapX = 512;
        int mapY = 256;
        float south = -90.0f;
        float north = 90.0f;
        float west = -180.0f;
        float east = 180.0f;
        Perlin myPerlin = new Perlin();
        Texture2D heightTexture, normalTexture;
        ModuleBase myModule = myPerlin;

        Noise2D heightMap;
        heightMap = new Noise2D(mapX, mapY, myModule);
        heightMap.GenerateSpherical(south, north, west, east);

        /* Get the heightMap, which is just a grayscale of the texture */
        heightTexture = heightMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
        height = heightTexture.EncodeToPNG();

        /* Get the normal map. */
        normalTexture = heightMap.GetNormalMap(0);
        normal = normalTexture.EncodeToPNG();


        //SAVE THE PICTURE
        SaveTexture(heightTexture);
    }
}
