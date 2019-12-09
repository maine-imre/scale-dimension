using Unity.Mathematics;
using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections
{

    public class CubeRenderer : MonoBehaviour
    {
        public Material mat;
        
        // Start is called before the first frame update
        void Start()
        {
            float3[] vertices = 
                GetComponentInParent<IMRE.ScaleDimension.CrossSections.CubeCrossSection>().cubeVertices;
            
            Mesh mesh = new Mesh();
            
            mesh.vertices = new Vector3[] {vertices[0], vertices[1], vertices[2], vertices[3], vertices[4], vertices[5], vertices[6], vertices[7]};

            int[] tris =
            {
                0, 1, 3, 
                3, 1, 2, 
                4, 5, 7, 
                7, 5, 6,
                3, 2, 7, 
                7, 3,6, 
                1, 0, 5, 
                5, 0, 4, 
                2, 1, 6, 
                6, 1, 5, 
                0, 3, 4, 
                4, 3, 7
                
            };

            mesh.triangles = tris;
            mesh.RecalculateNormals();

            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            gameObject.GetComponent<MeshRenderer>().material = mat;
            
        }

    }
}

