using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;


namespace IMRE.ScaleDimension.CrossSections
{
    public class TetrahderonCrossSection : UnityEngine.MonoBehaviour
    {

        public void crossSectTetrahedron(float3 point, float3 normalDirection, float3[] vertices,
            LineRenderer crossSectionRenderer, Mesh crossSectionMesh)
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
            triangle1[1] = c;
            triangle1[2] = d;
            
            float3[] triangle3 = new float3[3];
            triangle1[0] = a;
            triangle1[1] = d;
            triangle1[2] = b;

            float3[] triangle4 = new float3[3];
            triangle1[0] = b;
            triangle1[1] = c;
            triangle1[2] = d;

            float3 triangle1Normal = math.cross(Vector3.Normalize(c - a), Vector3.Normalize(b - a));
            float3 triangle2Normal = math.cross(Vector3.Normalize(c - a), Vector3.Normalize(d - a));
            float3 triangle3Normal = math.cross(Vector3.Normalize(d - a), Vector3.Normalize(b - a));
            float3 triangle4Normal = math.cross(Vector3.Normalize(c - b), Vector3.Normalize(d - b));
            
            float3 testpoint = new float3();
            float3 testdirection = new float3();

            intersectPlanes(testpoint, testdirection, triangle1Normal, a);
            intersectPlanes(testpoint, testdirection, triangle2Normal, a);
            intersectPlanes(testpoint, testdirection, triangle3Normal, a);
            intersectPlanes(testpoint, testdirection, triangle4Normal, b);

            TriangleCrossSection crossSection1 = new TriangleCrossSection();
            TriangleCrossSection crossSection2 = new TriangleCrossSection();
            TriangleCrossSection crossSection3 = new TriangleCrossSection();
            TriangleCrossSection crossSection4 = new TriangleCrossSection();

            crossSection1.crossSectTri(testpoint, testdirection, triangle1, crossSectionRenderer);
            crossSection2.crossSectTri(testpoint, testdirection, triangle2, crossSectionRenderer);
            crossSection3.crossSectTri(testpoint, testdirection, triangle3, crossSectionRenderer);
            crossSection4.crossSectTri(testpoint, testdirection, triangle4, crossSectionRenderer);
            
            //verts
            Vector3[] verts = new Vector3[4];
            verts[1] = ;
            verts[2] = ;
            verts[3] = ;
            verts[4] = ;

            crossSectionMesh.vertices = verts;
           
            //tris
            Vector3[] tris = ;
            crossSectionMesh.triangles = tris;

            //uvs
            
            //normals


        }

        /// <summary>
        /// function that calculates intersection of two planes, returns a line segment
        /// using mathematics described here http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <returns></returns>
        public float3[] intersectPlanes(float3 plane1Point, float3 normal1, float3 plane2Point, float3 normal2)
        {
            //TODO ^^^ the return can't be a float3, need to return a point and a direciton to describe a line.
            float3 u = Vector3.Normalize(math.cross(normal1, normal2));

            float3 lineDirInPlane2 = Vector3.Normalize(math.cross(normal2, u));

            //TODO check that the dot product is used correctly for projection
            float3 lineDirInPlane2_prime = lineDirInPlane2 - math.dot(normal1, lineDirInPlane2) * normal1;

            //TODO normalize this
            float3 p2_prime = Vector3.Normalize( plane2Point- math.dot(plane2Point - plane1Point, normal1) * normal1);
            
            //TODO intersect two lines, gives a point on desired line (with dir u)
            //Line 1: point = p2p_prime & dir = lineDirInPlane2_prime
            //Line 2: point = plane2point & dir = lineDirInPlane2
            
            float3[] result = new float3[2];
            //result[1] = point;
            result[2] = u;

            return result;
        }
    }
}