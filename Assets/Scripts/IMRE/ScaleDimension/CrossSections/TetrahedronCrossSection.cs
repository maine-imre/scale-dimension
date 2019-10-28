using UnityEngine;
using Unity.Mathematics;


namespace IMRE.ScaleDimension.CrossSections
{
    public class TetrahderonCrossSection : UnityEngine.MonoBehaviour
    {

        public void crossSectTetrahedron(float3 point, float3 normalDirection, float3[] vertices,
            LineRenderer crossSectionRenderer, MeshRenderer crossSectionMesh)
        {
            //take vertices and organize them into clockwise triangles to be passed to triangle intersection function
            float3 a = vertices[0];
            float3 b = vertices[1];
            float3 c = vertices[2];
            float3 d = vertices[3];
            
            float3[] triangle1 = new float3[3];
            triangle1[0] = a;
            triangle1[1] = b;
            triangle1[2] = c;
            
            float3[] triangle2 = new float3[3];
            triangle1[0] = a;
            triangle1[1] = d;
            triangle1[2] = b;
            
            float3[] triangle3 = new float3[3];
            triangle1[0] = a;
            triangle1[1] = c;
            triangle1[2] = d;
            
            float3[] triangle4 = new float3[3];
            triangle1[0] = b;
            triangle1[1] = c;
            triangle1[2] = d;

            float3 testpoint = new float3();
            float3 testdirection = new float3();

            TriangleCrossSection crossSection1 = new TriangleCrossSection();
            TriangleCrossSection crossSection2 = new TriangleCrossSection();
            TriangleCrossSection crossSection3 = new TriangleCrossSection();
            TriangleCrossSection crossSection4 = new TriangleCrossSection();
            
            crossSection1.crossSectTri(testpoint, testdirection, triangle1, crossSectionRenderer);
            crossSection2.crossSectTri(testpoint, testdirection, triangle2, crossSectionRenderer);
            crossSection3.crossSectTri(testpoint, testdirection, triangle3, crossSectionRenderer);
            crossSection4.crossSectTri(testpoint, testdirection, triangle4, crossSectionRenderer);
            
        }

        /// <summary>
        /// function that calculates intersection of two planes, returns a line segment
        /// using mathematics described here http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <returns></returns>
        public float3 intersectSurface()
        {
            
            return new float3();
        }
    }
}