using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections
{
    public class SquareRenderer : MonoBehaviour
    {
        public Material mat;

        
        // Start is called before the first frame update
        void Start()
        {
            Unity.Mathematics.float3x4 verticies =
                GetComponentInParent<IMRE.ScaleDimension.CrossSections.SquareCrossSection>().squareVertices;
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();

            lr.positionCount = 4;
            lr.SetPositions(new Vector3[] {verticies.c0, verticies.c1, verticies.c2, verticies.c3});
    
            lr.useWorldSpace = true;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.loop = true;
            
            lr.material = mat;

        }
    }
}
