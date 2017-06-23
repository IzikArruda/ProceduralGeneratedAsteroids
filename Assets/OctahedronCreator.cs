﻿using UnityEngine;
using System.Collections;

/*
 * Creates a octahedron.
 * 
 * Made using the tutorial from https://www.binpress.com/tutorial/creating-an-octahedron-sphere/162
 */
public class OctahedronCreator : MonoBehaviour {

    /* The stats of the octahedron */
    public int subDivisions;
    public float radius;

    /* A reference to each direction, used in handling quadrants of the sphere */
    private static Vector3[] directions = {
        Vector3.left,
        Vector3.back,
        Vector3.right,
        Vector3.forward
    };

    void Start() {
        /*  
         * On initlization, create the mesh 
         */

        /* Create the mesh of the octahedron */
        GetComponent<MeshFilter>().mesh = CreateOctahedron(subDivisions, radius);
    }


    public Mesh CreateOctahedron(int subDivisions, float radius) {
        /*
         * Return a mesh that forms an octahedron with the given divisions and radius
         */

        /* Clamp the subDivisions of the octahedron */
        if(subDivisions < 0) {
            subDivisions = 0;
            Debug.Log("Clamped subDivisions value up to 0");
        }
        else if(subDivisions > 6) {
            subDivisions = 6;
            Debug.Log("Clamped subDivisions value down to 6");
        }

        /* Get the resolution of the sphere */
        int resolution = 1 << subDivisions;
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1) * 4 -
            (resolution * 2 - 1) * 3];
        int[] triangles = new int[(1 << (subDivisions * 2 + 3)) * 3];

        /* Calculate the vertices and triangles of the sphere */
        CreateOctahedron(vertices, triangles, resolution);
        
        /* Create the tangent vectors for the sphere to be able to use bumb-maps */
        Vector4[] tangents = new Vector4[vertices.Length];
        CreateTangents(vertices, tangents);
        
        /* Get the normals of the vertices */
        Vector3[] normals = new Vector3[vertices.Length];
        Normalize(vertices, normals);

        /* Get the UV coordinates of the vertices */
        Vector2[] uv = new Vector2[vertices.Length];
        CreateUV(vertices, uv);


        /* Apply the radius to the sphere's vertices */
        if(radius != 1f) {
            for(int i = 0; i < vertices.Length; i++) {
                vertices[i] *= radius;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Octahedron Sphere";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.tangents = tangents;
        mesh.triangles = triangles;


        return mesh;
    }


    void CreateTangents(Vector3[] vertices, Vector4[] tangents) {
        /*
         * Create the tagent vectors for each vertice
         */

        for(int i = 0; i < vertices.Length; i++) {
            Vector3 v = vertices[i];
            v.y = 0f;
            v = v.normalized;
            Vector4 tangent;
            tangent.x = -v.z;
            tangent.y = 0f;
            tangent.z = v.x;
            tangent.w = -1f;
            tangents[i] = tangent;
        }

        /* Define the tangent vectors for the polar vertices */
        tangents[vertices.Length - 4] = tangents[0] = new Vector3(-1f, 0, -1f).normalized;
        tangents[vertices.Length - 3] = tangents[1] = new Vector3(1f, 0f, -1f).normalized;
        tangents[vertices.Length - 2] = tangents[2] = new Vector3(1f, 0f, 1f).normalized;
        tangents[vertices.Length - 1] = tangents[3] = new Vector3(-1f, 0f, 1f).normalized;
        for(int i = 0; i < 4; i++) {
            tangents[vertices.Length - 1 - i].w = tangents[i].w = -1f;
        }
    }


    void CreateOctahedron(Vector3[] vertices, int[] triangles, int resolution) {
        /*
         * Create the vertices that form the Octahedron along with it's normals.
         */

        /* The current vertex index */
        int v = 0;
        /* The current triangle strip index */
        int vBottom = 0;
        int t= 0;


        /* Iteratively define the first bottom vertices */
        for(int i = 0; i < 4; i++) {
            vertices[v++] = Vector3.down;
        }

        /* Loop through rows of vertices of the lower hemisphere */
        for(int i = 1; i <= resolution; i++) {
            float progress = (float) i / resolution;
            Vector3 from, to;
            vertices[v++] = to = Vector3.Lerp(Vector3.down, Vector3.forward, progress);
            for(int d = 0; d < 4; d++) {
                from = to;
                to = Vector3.Lerp(Vector3.down, directions[d], progress);
                t = CreateLowerStrip(i, v, vBottom, t, triangles);
                v = CreateVertexLine(from, to, i, v, vertices);
                vBottom += i > 1 ? (i - 1) : 1;
            }
            vBottom = v - 1 - i * 4;
        }

        /* Loop through rows of vertices of the upper hemisphere */
        for(int i = resolution - 1; i >= 1; i--) {
            float progress = (float) i / resolution;
            Vector3 from, to;
            vertices[v++] = to = Vector3.Lerp(Vector3.up, Vector3.forward, progress);
            for(int d = 0; d < 4; d++) {
                from = to;
                to = Vector3.Lerp(Vector3.up, directions[d], progress);
                t = CreateUpperStrip(i, v, vBottom, t, triangles);
                v = CreateVertexLine(from, to, i, v, vertices);
                vBottom += i + 1;
            }
            vBottom = v - 1 - i * 4;
        }

        /* finish by setting the top row */
        for(int i = 0; i < 4; i++) {
            triangles[t++] = vBottom;
            triangles[t++] = v;
            triangles[t++] = ++vBottom;
            vertices[v++] = Vector3.up;
        }
    }


    private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] triangles) {
        /*
         * Create a triangle strip on the bottom face of the vertices defined by triangles[t]
         */

        for(int i = 1; i < steps; i++) {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;

            triangles[t++] = vBottom++;
            triangles[t++] = vTop++;
            triangles[t++] = vBottom;
        }
        triangles[t++] = vBottom;
        triangles[t++] = vTop - 1;
        triangles[t++] = vTop;

        return t;
    }
    private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] triangles) {
        /*
         * Create a triangle strip on the upper face of the vertices defined by triangles[t]
         */
        triangles[t++] = vBottom;
        triangles[t++] = vTop - 1;
        triangles[t++] = ++vBottom;
        for(int i = 1; i <= steps; i++) {
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            triangles[t++] = vBottom;

            triangles[t++] = vBottom;
            triangles[t++] = vTop++;
            triangles[t++] = ++vBottom;
        }
        return t;
    }


    private static int CreateVertexLine(
    Vector3 from, Vector3 to, int steps, int v, Vector3[] vertices) {
        /*
         * Return the next vertex index as we add vertices to the vertices list
         */

        for(int i = 1; i <= steps; i++) {
            vertices[v++] = Vector3.Lerp(from, to, (float) i / steps);
        }

        return v;
    }

    void Normalize(Vector3[] vertices, Vector3[] normals) {
        /*
         * Get the normalized vectors of the given vertices
         */

        for(int i = 0; i < vertices.Length; i++) {
            normals[i] = vertices[i] = vertices[i].normalized;
        }
    }


    void CreateUV(Vector3[] vertices, Vector2[] uv) {
        /*
         * Calculate the UVs by converting the given vertices
         */
        float previousX = 1f;


        for(int i = 0; i < vertices.Length; i++) {
            Vector3 v = vertices[i];
            /* Check if we passed a seam */
            if(v.x == previousX) {
                uv[i - 1].x = 1f;
            }
            previousX = v.x;
            Vector2 textureCoordinates;
            textureCoordinates.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);
            if(textureCoordinates.x < 0f) {
                textureCoordinates.x += 1f;
            }
            textureCoordinates.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
            uv[i] = textureCoordinates;
        }

        /* Adjust the horizontal position of the polar vertices once the UVs have been calculated. */
        uv[vertices.Length - 4].x = uv[0].x = 0.125f;
        uv[vertices.Length - 3].x = uv[1].x = 0.375f;
        uv[vertices.Length - 2].x = uv[2].x = 0.625f;
        uv[vertices.Length - 1].x = uv[3].x = 0.875f;
    }
}
