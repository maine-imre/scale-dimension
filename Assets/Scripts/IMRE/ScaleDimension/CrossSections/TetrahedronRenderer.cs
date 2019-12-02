using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections
{
    public class TetrahedronRenderer : MonoBehaviour
    {
        public Material mat;
        // Start is called before the first frame update
        void Start()
        {
            Unity.Mathematics.float3x4 vertices =
                GetComponentInParent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().tetrahderonVertices;
            Mesh mesh = new Mesh();

            mesh.vertices = new Vector3[] {vertices.c0, vertices.c1, vertices.c2, vertices.c3};

            int[] tris =
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 1,
                1, 2, 3
            };

            mesh.triangles = tris;
            mesh.RecalculateNormals();

            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().material = mat;

        }
    }
}
