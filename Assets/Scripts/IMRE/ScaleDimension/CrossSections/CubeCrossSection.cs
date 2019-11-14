using UnityEngine;
using Unity.Mathematics;

namespace IMRE.ScaleDimension.CrossSections
{
    public class CubeCrossSection : UnityEngine.MonoBehaviour
    {
        public void crossSectCube(Unity.Mathematics.float3 point, Unity.Mathematics.float3 direction,
            float3[] vertices, UnityEngine.LineRenderer cubeRenderer, Mesh cubeMesh)
        {
            //8 vertices for cube
            float3 a = vertices[0];
            float3 b = vertices[1];
            float3 c = vertices[2];
            float3 d = vertices[3];
            float3 e = vertices[4];
            float3 f = vertices[5];
            float3 g = vertices[7];
            float3 h = vertices[8];
            
            //different faces of cube arranged in clockwise manner
            float3[] topFace = {a, b, c, d};
            float3[] bottomFace = {e, f, g, h};
            float3[] frontFace = {d, c, g, h};
            float3[] backFace = {b, a, e, f};
            float3[] rightFace = {c, b, f, g};
            float3[] leftFace = {a, d, h, e};

            //normals for faces of cubes; parallel faces will have same normal
            float3 topBottomNormal = math.cross(Vector3.Normalize(b - a), Vector3.Normalize(d - a));
            float3 frontBackNormal = math.cross(Vector3.Normalize(c - d), Vector3.Normalize(h - d));
            float3 rightLeftNormal = math.cross(Vector3.Normalize(b - c), Vector3.Normalize(g - c));
            
            float3 testpoint = new float3();
            float3 testdirection = new float3();

            float3 topPoint = intersectPlanes(testpoint, testdirection, a, topBottomNormal)[0];
            float3 topDir = intersectPlanes(testpoint, testdirection, a, topBottomNormal)[1];

            float3 bottomPoint = intersectPlanes(testpoint, testdirection, e, topBottomNormal)[0];
            float3 bottomDir = intersectPlanes(testpoint, testdirection, e, topBottomNormal)[1];

            float3 frontPoint = intersectPlanes(testpoint, testdirection, d, frontBackNormal)[0];
            float3 fronDir = intersectPlanes(testpoint, testdirection, d, frontBackNormal)[1];
            
            float3 backPoint = intersectPlanes(testpoint, testdirection, b, frontBackNormal)[0];
            float3 backDir = intersectPlanes(testpoint, testdirection, b, frontBackNormal)[1];
            
            float3 rightPoint = intersectPlanes(testpoint, testdirection, c, rightLeftNormal)[0];
            float3 rightDir = intersectPlanes(testpoint, testdirection, c, rightLeftNormal)[1];

            float3 leftPoint = intersectPlanes(testpoint, testdirection, a, rightLeftNormal)[0];
            float3 leftDir = intersectPlanes(testpoint, testdirection, a, rightLeftNormal)[1];


            
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
            float3 p2_prime = Vector3.Normalize(plane2Point - math.dot(plane2Point - plane1Point, normal1) * normal1);

            //TODO intersect two lines, gives a point on desired line (with dir u)
            float3 point = p2_prime;
            float3 dir = lineDirInPlane2_prime;
            //Line 2: point = plane2point & dir = lineDirInPlane2

            float3[] result = new float3[2];
            result[0] = point;
            result[1] = dir;

            return result;
        }
    }
}