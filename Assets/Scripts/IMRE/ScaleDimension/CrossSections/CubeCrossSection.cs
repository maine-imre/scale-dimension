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
            transform.position = Vector3.zero;
        
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
            BuildSquare(square1, planePos, planeNormal);
            BuildSquare(square2, planePos, planeNormal);
            BuildSquare(square3, planePos, planeNormal);
            BuildSquare(square4, planePos, planeNormal);
            BuildSquare(square5, planePos, planeNormal);
            BuildSquare(square6, planePos, planeNormal);
            
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
            float3 g = vertices[6];
            float3 h = vertices[7];
            
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

            LineRenderer crossSectionRenderer1 = squareXC[0].GetComponent<LineRenderer>();
            LineRenderer crossSectionRenderer2 = squareXC[1].GetComponent<LineRenderer>();
            LineRenderer crossSectionRenderer3 = squareXC[2].GetComponent<LineRenderer>();
            LineRenderer crossSectionRenderer4 = squareXC[3].GetComponent<LineRenderer>();
            LineRenderer crossSectionRenderer5 = squareXC[4].GetComponent<LineRenderer>();
            LineRenderer crossSectionRenderer6 = squareXC[5].GetComponent<LineRenderer>();

            int vertCount = 0;
            
            bool line1 = false, line2 = false, line3 = false, line4 = false, line5 = false, line6 = false;

            if (crossSectionRenderer1.enabled == true)
            {
                vertCount++;
                line1 = true;
            }
            
            if (crossSectionRenderer2.enabled == true)
            {
                vertCount++;
                line2 = true;
            }
            
            if (crossSectionRenderer3.enabled == true)
            {
                vertCount++;
                line3 = true;
            }
            
            if (crossSectionRenderer4.enabled == true)
            {
                vertCount++;
                line4 = true;
            }
            
            if (crossSectionRenderer5.enabled == true)
            {
                vertCount++;
                line5 = true;
            }
            
            if (crossSectionRenderer6.enabled == true)
            {
                vertCount++;
                line6 = true;
            }

            if (vertCount == 3)
            {
                Vector3[] verts = new Vector3[3];
                
                if (line1 && line5 && line3)
                {
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer5.GetPosition(1) == verts[0] ||
                        crossSectionRenderer5.GetPosition(1) == verts[1]
                            ? crossSectionRenderer5.GetPosition(0)
                            : crossSectionRenderer5.GetPosition(1);
                }
                else if (line1 && line3 && line6)
                {
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer3.GetPosition(1) == verts[0] ||
                        crossSectionRenderer3.GetPosition(1) == verts[1]
                            ? crossSectionRenderer3.GetPosition(0)
                            : crossSectionRenderer3.GetPosition(1);
                }
                else if (line1 && line6 && line4)
                {
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer6.GetPosition(1) == verts[0] ||
                        crossSectionRenderer6.GetPosition(1) == verts[1]
                            ? crossSectionRenderer6.GetPosition(0)
                            : crossSectionRenderer6.GetPosition(1);
                }
                else if (line1 && line4 && line5)
                {
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer4.GetPosition(1) == verts[0] ||
                        crossSectionRenderer4.GetPosition(1) == verts[1]
                            ? crossSectionRenderer4.GetPosition(0)
                            : crossSectionRenderer4.GetPosition(1);
                }
                else if (line2 && line3 && line5)
                {
                    verts[0] = crossSectionRenderer2.GetPosition(0);
                    verts[1] = crossSectionRenderer2.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer3.GetPosition(1) == verts[0] ||
                        crossSectionRenderer3.GetPosition(1) == verts[1]
                            ? crossSectionRenderer3.GetPosition(0)
                            : crossSectionRenderer3.GetPosition(1);
                }
                else if (line2 && line6 && line3)
                {
                    verts[0] = crossSectionRenderer2.GetPosition(0);
                    verts[1] = crossSectionRenderer2.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer6.GetPosition(1) == verts[0] ||
                        crossSectionRenderer6.GetPosition(1) == verts[1]
                            ? crossSectionRenderer6.GetPosition(0)
                            : crossSectionRenderer6.GetPosition(1);
                }
                else if (line2 && line4 && line6)
                {
                    verts[0] = crossSectionRenderer2.GetPosition(0);
                    verts[1] = crossSectionRenderer2.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer4.GetPosition(1) == verts[0] ||
                        crossSectionRenderer4.GetPosition(1) == verts[1]
                            ? crossSectionRenderer4.GetPosition(0)
                            : crossSectionRenderer4.GetPosition(1);
                }
                else if (line2 && line5 && line4)
                {
                    verts[0] = crossSectionRenderer2.GetPosition(0);
                    verts[1] = crossSectionRenderer2.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer5.GetPosition(1) == verts[0] ||
                        crossSectionRenderer5.GetPosition(1) == verts[1]
                            ? crossSectionRenderer5.GetPosition(0)
                            : crossSectionRenderer5.GetPosition(1);
                }
            }
            else if (vertCount == 4)
            {
                Vector3[] verts = new Vector3[4];
                if (line4 && line5 && line3 && line6)
                {
                    verts[0] = crossSectionRenderer5.GetPosition(0);
                    verts[1] = crossSectionRenderer5.GetPosition(1);
                    verts[2] =
                        (crossSectionRenderer3.GetPosition(1) == verts[0] ||
                         crossSectionRenderer3.GetPosition(1) == verts[1])
                            ? crossSectionRenderer3.GetPosition(0)
                            : crossSectionRenderer3.GetPosition(1);
                    verts[3] =
                        (crossSectionRenderer6.GetPosition(1) == verts[0] ||
                         crossSectionRenderer6.GetPosition(1) == verts[1] ||
                         crossSectionRenderer6.GetPosition(1) == verts[2])
                            ? crossSectionRenderer6.GetPosition(0)
                            : crossSectionRenderer6.GetPosition(1);

                    cubeMesh.vertices = verts;                   
                }   
                else if (line1 && line5 && line2 && line6)
                {
                    verts[0] = crossSectionRenderer5.GetPosition(0);
                    verts[1] = crossSectionRenderer5.GetPosition(1);
                    verts[2] =
                        (crossSectionRenderer2.GetPosition(1) == verts[0] ||
                         crossSectionRenderer2.GetPosition(1) == verts[1])
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
                    verts[3] =
                        (crossSectionRenderer6.GetPosition(1) == verts[0] ||
                         crossSectionRenderer6.GetPosition(1) == verts[1] ||
                         crossSectionRenderer6.GetPosition(1) == verts[2])
                            ? crossSectionRenderer6.GetPosition(0)
                            : crossSectionRenderer6.GetPosition(1);

                    cubeMesh.vertices = verts;
                }
                else if (line1 && line5 && line2 && line3)
                {
                    verts[0] = crossSectionRenderer5.GetPosition(0);
                    verts[1] = crossSectionRenderer5.GetPosition(1);
                    verts[2] =
                        (crossSectionRenderer2.GetPosition(1) == verts[0] ||
                         crossSectionRenderer2.GetPosition(1) == verts[1])
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
                    verts[3] =
                        (crossSectionRenderer3.GetPosition(1) == verts[0] ||
                         crossSectionRenderer3.GetPosition(1) == verts[1] ||
                         crossSectionRenderer3.GetPosition(1) == verts[2])
                            ? crossSectionRenderer3.GetPosition(0)
                            : crossSectionRenderer3.GetPosition(1);

                    cubeMesh.vertices = verts;
                }
                else if (line1 && line3 && line2 && line6)
                {
                    verts[0] = crossSectionRenderer3.GetPosition(0);
                    verts[1] = crossSectionRenderer3.GetPosition(1);
                    verts[2] =
                        (crossSectionRenderer2.GetPosition(1) == verts[0] ||
                         crossSectionRenderer2.GetPosition(1) == verts[1])
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
                    verts[3] =
                        (crossSectionRenderer6.GetPosition(1) == verts[0] ||
                         crossSectionRenderer6.GetPosition(1) == verts[1] ||
                         crossSectionRenderer6.GetPosition(1) == verts[2])
                            ? crossSectionRenderer6.GetPosition(0)
                            : crossSectionRenderer6.GetPosition(1);

                    cubeMesh.vertices = verts;
                }
                else if (line1 && line6 && line2 && line4)
                {
                    verts[0] = crossSectionRenderer6.GetPosition(0);
                    verts[1] = crossSectionRenderer6.GetPosition(1);
                    verts[2] =
                        (crossSectionRenderer2.GetPosition(1) == verts[0] ||
                         crossSectionRenderer2.GetPosition(1) == verts[1])
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
                    verts[3] =
                        (crossSectionRenderer4.GetPosition(1) == verts[0] ||
                         crossSectionRenderer4.GetPosition(1) == verts[1] ||
                         crossSectionRenderer4.GetPosition(1) == verts[2])
                            ? crossSectionRenderer4.GetPosition(0)
                            : crossSectionRenderer4.GetPosition(1);

                    cubeMesh.vertices = verts;
                }
                else if (line1 && line4 && line2 && line5)
                {
                    verts[0] = crossSectionRenderer4.GetPosition(0);
                    verts[1] = crossSectionRenderer4.GetPosition(1);
                    verts[2] =
                        (crossSectionRenderer2.GetPosition(1) == verts[0] ||
                         crossSectionRenderer2.GetPosition(1) == verts[1])
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
                    verts[3] =
                        (crossSectionRenderer5.GetPosition(1) == verts[0] ||
                         crossSectionRenderer5.GetPosition(1) == verts[1] ||
                         crossSectionRenderer5.GetPosition(1) == verts[2])
                            ? crossSectionRenderer5.GetPosition(0)
                            : crossSectionRenderer5.GetPosition(1);

                    cubeMesh.vertices = verts;
                }
                
                //tris
                //find the opposite corner to #0
                //assume 0,1,2,3
                float3 cross0 = IMRE.Math.Operations.AngleCrossProduct(verts, 3, 0, 1);
                //float3 cross1 = Unity.Mathematics.math.cross(verts[3] - verts[0], verts[1] - verts[0]);
                //float3 cross1 = Unity.Mathematics.math.cross(verts[a] - verts[b], verts[c] - verts[b]);
                float3 cross1 = IMRE.Math.Operations.AngleCrossProduct(verts, 0, 1, 2);
                float3 cross2 = IMRE.Math.Operations.AngleCrossProduct(verts, 1, 2, 3);
                float3 cross3 = IMRE.Math.Operations.AngleCrossProduct(verts, 2, 3, 0);
                
                int[] tris = {0, 1, 2, 0, 2, 3};
                if (!(cross0.Equals(cross1) && cross1.Equals(cross2) && cross2.Equals(cross3)))
                { 
                    //assume 0,1,3,2
                   cross0 = IMRE.Math.Operations.AngleCrossProduct(verts, 2, 0, 1);
                   cross1 = IMRE.Math.Operations.AngleCrossProduct(verts, 0, 1, 3);
                   cross2 = IMRE.Math.Operations.AngleCrossProduct(verts, 1, 3, 2);
                   cross3 = IMRE.Math.Operations.AngleCrossProduct(verts, 3, 2, 0);
                   
                   tris = new int[] {0, 1, 3, 0, 3, 2};
                   
                   if (!(cross0.Equals(cross1) && cross1.Equals(cross2) && cross2.Equals(cross3)))
                   {
                       //assume 0,2,1,3 (switch 1 and 2)
                       cross0 = IMRE.Math.Operations.AngleCrossProduct(verts, 3, 0, 2);
                       cross1 = IMRE.Math.Operations.AngleCrossProduct(verts, 0, 2, 1);
                       cross2 = IMRE.Math.Operations.AngleCrossProduct(verts, 2, 1, 3);
                       cross3 = IMRE.Math.Operations.AngleCrossProduct(verts, 1, 3, 0);
                       if (!(cross0.Equals(cross1) && cross1.Equals(cross2) && cross2.Equals(cross3)))
                       {
                           UnityEngine.Debug.LogWarning("failed check on vertex ordering");
                       }
                       tris = new int[] {0, 2, 1, 0, 1, 3};

                   }

                }  
                cubeMesh.triangles = tris;     
            }
            else if (vertCount == 5)
            {
                Vector3[] verts = new Vector3[5];
                
            }
            else if (vertCount == 6)
            {
                Vector3[] verts = new Vector3[6];
                
            }

            

        }
        
    }
}