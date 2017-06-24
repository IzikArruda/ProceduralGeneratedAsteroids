using UnityEngine;
using System.Collections;


/*
 * Creates randomnly generated asteroids using a Octahedron creator and a perlin noise texture.
 * 
 * The transform that holds this script will be used as the container for all it's generated asteroids.
 */
public class AsteroidGenerator : MonoBehaviour {


    /* A helper script that is used to create the base mesh of an asteroid */
    public OctahedronCreator octahedronCreator;

    /* A helper script that is used to create perlin noise to shape the asteroid and it's texture */
    public NoiseGenerator noiseGenerator;




    /* This will be put into the asteoir dscript. The byte array of the asteroid's texture */
    /* The maps used for the asteroids texture */
    private byte[] asteroidHeightBytes;
    private byte[] asteroidNormalBytes;



    void Start () {
        /* Create an asteroid */
        CreateAsteroid();
	}


    void CreateAsteroid() {
        /*
         * Create an asteroid step by step.
         */
        GameObject asteroid = new GameObject();

        /* Name the asteroid and link it to it's container */
        asteroid.name = "Generated Asteroid";
        asteroid.transform.parent = transform;

        /* Add a mesh filter and a mesh renderer to the asteroid */
        asteroid.AddComponent<MeshFilter>();
        asteroid.AddComponent<MeshRenderer>();

        /* Create the mesh of the asteroid */
        CreateAsteroidMesh(asteroid);

        /* Create the asteroid's material and texture */
        CreateAsteroidMaterial(asteroid);
    }


    void CreateAsteroidMesh(GameObject asteroid) {
        /*
         * Create an octahedron mesh for the asteroid's meshFilter.
         */
        int subDivisions = 4;
        float radius = 2;

        asteroid.GetComponent<MeshFilter>().mesh = octahedronCreator.CreateOctahedron(subDivisions, radius);
    }


    void CreateAsteroidMaterial(GameObject asteroid) {
        /*
         * Create a material and a texture to link to the asteroid. Assumes a meshRenderer is already attached.
         */
        Texture2D asteroidTexture = new Texture2D(0, 0);
        Texture2D asteroidHeightMap = new Texture2D(0, 0);
        Texture2D asteroidNormalMap = new Texture2D(0, 0);

        /* Create the basic material to be applied to the asteroid */
        Material asteroidMaterial = new Material(Shader.Find("Standard"));
        asteroidMaterial.EnableKeyword("_NORMALMAP");
        asteroidMaterial.EnableKeyword("_PARALLAXMAP");

        /* Get a texture2D of random perlin noise */
        noiseGenerator.CreateSphericalPerlinTexture(ref asteroidHeightBytes, ref asteroidNormalBytes);
        asteroidHeightMap.LoadImage(asteroidHeightBytes);
        asteroidNormalMap.LoadImage(asteroidNormalBytes);
        
        /* Set the heightMap and the normalMap the material */
        asteroidMaterial.mainTexture = asteroidHeightMap;
        asteroidMaterial.SetTexture("_ParallaxMap", asteroidHeightMap);
        //normal map looks wierd
        //asteroidMaterial.SetTexture("_BumpMap", asteroidNormalMap);
        
        asteroid.GetComponent<MeshRenderer>().material = asteroidMaterial;
    }
}
