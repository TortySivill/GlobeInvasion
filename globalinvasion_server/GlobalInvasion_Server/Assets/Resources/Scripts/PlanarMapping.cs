/* PlanarMapping.cs - Generate Planar UV mapping for a mesh */

using UnityEngine;

public class PlanarMapping : MonoBehaviour {

    void Start() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];
        
        for (int i = 0; i < uvs.Length; i++)
            uvs[i].Set(0.5f + (vertices[i].x / bounds.size.x), 0.5f + (vertices[i].z / bounds.size.z));

        mesh.uv = uvs;
    }

}
