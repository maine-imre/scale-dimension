using System;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IMRE.ScaleDimension.CrossSections
{
    
    public class CubeCrossSection : AbstractCrossSection
    {        
        public float3[] cubeVertices = new float3[8];
        private List<SquareCrossSection> squareXC;

        private void Start()
        {
            //using same orientation as cross section function
            Unity.Mathematics.float3 a = cubeVertices[0];
            Unity.Mathematics.float3 b = cubeVertices[1];
            Unity.Mathematics.float3 c = cubeVertices[2];
            Unity.Mathematics.float3 d = cubeVertices[3];
            Unity.Mathematics.float3 e = cubeVertices[4];
            Unity.Mathematics.float3 f = cubeVertices[5];
            Unity.Mathematics.float3 g = cubeVertices[6];
            Unity.Mathematics.float3 h = cubeVertices[7];
            
            float3x4 square1 = new float3x4(a, b, c, d);
            float3x4 square2 = new float3x4(e, f, g, h);
            float3x4 square3 = new float3x4(d, c, g, h);
            float3x4 square4 = new float3x4(b, a, e, f);
            float3x4 square5 = new float3x4(c, b, f, g);
            float3x4 square6 = new float3x4(a, d, h, e);
            
            squareXC = new List<SquareCrossSection>();

            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>();
            gameObject.GetComponent<MeshRenderer>().material = mat;
        }

        private void Update()
        {
            updateSquares();
            crossSectCube(planePos, planeNormal, cubeVertices, GetComponent<MeshFilter>().mesh);
        }

        private void BuildSquare(float3x4 vertices, float3 planePos, float3 planeNorm)
        {
            GameObject square1_go = new GameObject();
            square1_go.transform.parent = this.transform;
            square1_go.AddComponent<SquareCrossSection>();
            square1_go.GetComponent<SquareCrossSection>().squareVertices = vertices;
            square1_go.GetComponent<SquareCrossSection>().planePos = planePos;
            square1_go.GetComponent<SquareCrossSection>().planeNormal = planeNorm;
            square1_go.GetComponent<SquareCrossSection>().mat = mat;
        }

        private void updateSquares()
        {
            Unity.Mathematics.float3 a = cubeVertices[0];
            Unity.Mathematics.float3 b = cubeVertices[1];
            Unity.Mathematics.float3 c = cubeVertices[2];
            Unity.Mathematics.float3 d = cubeVertices[3];
            Unity.Mathematics.float3 e = cubeVertices[4];
            Unity.Mathematics.float3 f = cubeVertices[5];
            Unity.Mathematics.float3 g = cubeVertices[6];
            Unity.Mathematics.float3 h = cubeVertices[7];
            
            float3x4 square1 = new float3x4(a, b, c, d);
            float3x4 square2 = new float3x4(e, f, g, h);
            float3x4 square3 = new float3x4(d, c, g, h);
            float3x4 square4 = new float3x4(b, a, e, f);
            float3x4 square5 = new float3x4(c, b, f, g);
            float3x4 square6 = new float3x4(a, d, h, e);
            
            squareXC.ForEach(p => p.planePos = planePos);
            squareXC.ForEach(p => p.planeNormal = planeNormal);

            squareXC[0].squareVertices = square1;
            squareXC[1].squareVertices = square2;
            squareXC[2].squareVertices = square3;
            squareXC[3].squareVertices = square4;
            squareXC[4].squareVertices = square5;
            squareXC[5].squareVertices = square6;

        }


        public void crossSectCube(Unity.Mathematics.float3 point, Unity.Mathematics.float3 direction,
            float3[] vertices, Mesh cubeMesh)
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