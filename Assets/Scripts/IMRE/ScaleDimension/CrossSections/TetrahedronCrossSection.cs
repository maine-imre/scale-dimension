using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections
{
    public class TetrahedronCrossSection : AbstractCrossSection
    {        
        public Unity.Mathematics.float3x4 tetrahderonVertices;
        private System.Collections.Generic.List<TriangleCrossSection> triXC;

        private void Start()
        {
            transform.position = UnityEngine.Vector3.zero;
            
            //take vertices and organize them into clockwise triangles to be passed to triangle intersection function
            Unity.Mathematics.float3 a = tetrahderonVertices[0];
            Unity.Mathematics.float3 b = tetrahderonVertices[1];
            Unity.Mathematics.float3 c = tetrahderonVertices[2];
            Unity.Mathematics.float3 d = tetrahderonVertices[3];

            Unity.Mathematics.float3x3 triangle1 = new Unity.Mathematics.float3x3(a, b, c);
            Unity.Mathematics.float3x3 triangle2 =  new Unity.Mathematics.float3x3(a, c, d);
            Unity.Mathematics.float3x3 triangle3 =  new Unity.Mathematics.float3x3(a, d, b);
            Unity.Mathematics.float3x3 triangle4 =  new Unity.Mathematics.float3x3(b, c, d);

            triXC = new System.Collections.Generic.List<TriangleCrossSection>();
            BuildTriangle(triangle1, planePos, planeNormal);
            BuildTriangle(triangle2, planePos, planeNormal);
            BuildTriangle(triangle3, planePos, planeNormal);
            BuildTriangle(triangle4, planePos, planeNormal);

            gameObject.AddComponent<UnityEngine.MeshRenderer>();
            gameObject.AddComponent<UnityEngine.MeshFilter>();
            gameObject.GetComponent<MeshRenderer>().material = mat;
        }

        private void Update()
        {
            UpdateTriangles();
            crossSectTetrahedron(planePos,planeNormal,tetrahderonVertices,
                GetComponent<UnityEngine.MeshFilter>().mesh);
        }

        private void BuildTriangle(Unity.Mathematics.float3x3 vertices, Unity.Mathematics.float3 planePos,
            Unity.Mathematics.float3 planeNorm)
        {
            UnityEngine.GameObject tri1_go = new UnityEngine.GameObject();
            tri1_go.transform.parent = this.transform;
            tri1_go.AddComponent<TriangleCrossSection>();
            triXC.Add(tri1_go.GetComponent<TriangleCrossSection>());
            tri1_go.GetComponent<TriangleCrossSection>().triangleVerticies = vertices;
            tri1_go.GetComponent<TriangleCrossSection>().planePos = planePos;
            tri1_go.GetComponent<TriangleCrossSection>().planeNormal = planeNorm;
            tri1_go.GetComponent<TriangleCrossSection>().mat = mat;
        }

        private void UpdateTriangles()
        {
            //take vertices and organize them into clockwise triangles to be passed to triangle intersection function
            Unity.Mathematics.float3 a = tetrahderonVertices[0];
            Unity.Mathematics.float3 b = tetrahderonVertices[1];
            Unity.Mathematics.float3 c = tetrahderonVertices[2];
            Unity.Mathematics.float3 d = tetrahderonVertices[3];

            Unity.Mathematics.float3x3 triangle1 = new Unity.Mathematics.float3x3(a, b, c);
            Unity.Mathematics.float3x3 triangle2 =  new Unity.Mathematics.float3x3(a, c, d);
            Unity.Mathematics.float3x3 triangle3 =  new Unity.Mathematics.float3x3(a, d, b);
            Unity.Mathematics.float3x3 triangle4 =  new Unity.Mathematics.float3x3(b, c, d);
            
            triXC.ForEach(p => p.planePos = planePos);
            triXC.ForEach(p => p.planeNormal = planeNormal);
            triXC[0].triangleVerticies = triangle1;
            triXC[1].triangleVerticies = triangle2;
            triXC[2].triangleVerticies = triangle3;
            triXC[3].triangleVerticies = triangle4;   
        }

        public void crossSectTetrahedron(Unity.Mathematics.float3 point, Unity.Mathematics.float3 normalDirection,
            Unity.Mathematics.float3x4 vertices, UnityEngine.Mesh tetrahedronMesh)
        {
            //take vertices and organize them into clockwise triangles to be passed to triangle intersection function
            Unity.Mathematics.float3 a = vertices.c0;
            Unity.Mathematics.float3 b = vertices.c1;
            Unity.Mathematics.float3 c = vertices.c2;
            Unity.Mathematics.float3 d = vertices.c3;

            Unity.Mathematics.float3[] triangle1 = {a, b, c};
            Unity.Mathematics.float3[] triangle2 = {a, c, d};
            Unity.Mathematics.float3[] triangle3 = {a, d, b};
            Unity.Mathematics.float3[] triangle4 = {b, c, d};

            Unity.Mathematics.float3 triangle1point = a;
            Unity.Mathematics.float3 triangle2point = a;
            Unity.Mathematics.float3 triangle3point = a;
            Unity.Mathematics.float3 triangle4point = b;

            Unity.Mathematics.float3 triangle1Normal =
                Unity.Mathematics.math.cross(UnityEngine.Vector3.Normalize(c - a),
                    UnityEngine.Vector3.Normalize(b - a));
            Unity.Mathematics.float3 triangle2Normal =
                Unity.Mathematics.math.cross(UnityEngine.Vector3.Normalize(c - a),
                    UnityEngine.Vector3.Normalize(d - a));
            Unity.Mathematics.float3 triangle3Normal =
                Unity.Mathematics.math.cross(UnityEngine.Vector3.Normalize(d - a),
                    UnityEngine.Vector3.Normalize(b - a));
            Unity.Mathematics.float3 triangle4Normal =
                Unity.Mathematics.math.cross(UnityEngine.Vector3.Normalize(c - b),
                    UnityEngine.Vector3.Normalize(d - b));

            //TODO make this pretty
            UnityEngine.LineRenderer crossSectionRenderer1 = triXC[0].GetComponent<UnityEngine.LineRenderer>();
            UnityEngine.LineRenderer crossSectionRenderer2 = triXC[1].GetComponent<UnityEngine.LineRenderer>();
            UnityEngine.LineRenderer crossSectionRenderer3 = triXC[2].GetComponent<UnityEngine.LineRenderer>();
            UnityEngine.LineRenderer crossSectionRenderer4 = triXC[3].GetComponent<UnityEngine.LineRenderer>();

            int vertCount = 0;

            bool line1 = false, line2 = false, line3 = false, line4 = false;

            if (crossSectionRenderer1.enabled)
            {
                vertCount++;
                line1 = true;
            }

            if (crossSectionRenderer2.enabled)
            {
                vertCount++;
                line2 = true;
            }

            if (crossSectionRenderer3.enabled)
            {
                vertCount++;
                line3 = true;
            }

            if (crossSectionRenderer4.enabled)
            {
                vertCount++;
                line4 = true;
            }

            //Intersection is Triangle
            if (vertCount == 3)
            {
                //verts
                UnityEngine.Vector3[] verts = new UnityEngine.Vector3[3];

                if (line1 && line2 && line3)
                {
                    //a way to avoid checking orientation of line segments
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer2.GetPosition(1) == verts[0] ||
                        crossSectionRenderer2.GetPosition(1) == verts[1]
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
                    
                }

                else if (line1 && line2 && line4)
                {
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer2.GetPosition(1) == verts[0] ||
                        crossSectionRenderer2.GetPosition(1) == verts[1]
                            ? crossSectionRenderer2.GetPosition(0)
                            : crossSectionRenderer2.GetPosition(1);
 
                }

                else if (line1 && line3 && line4)
                {
                    verts[0] = crossSectionRenderer1.GetPosition(0);
                    verts[1] = crossSectionRenderer1.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer3.GetPosition(1) == verts[0] ||
                        crossSectionRenderer3.GetPosition(1) == verts[1]
                            ? crossSectionRenderer3.GetPosition(0)
                            : crossSectionRenderer3.GetPosition(1);

                }

                else if (line2 && line3 && line4)
                { 
                    verts[0] = crossSectionRenderer2.GetPosition(0);
                    verts[1] = crossSectionRenderer2.GetPosition(1);
                    verts[2] =
                        crossSectionRenderer3.GetPosition(1) == verts[0] ||
                        crossSectionRenderer3.GetPosition(1) == verts[1]
                            ? crossSectionRenderer3.GetPosition(0)
                            : crossSectionRenderer3.GetPosition(1);

                }
                tetrahedronMesh.vertices = verts;

                //tris
                int[] tris = {0, 1, 2};
                tetrahedronMesh.triangles = tris;
            }
            //Intersection is quadrilateral
            else if (vertCount == 4)
            {
                //verts
                UnityEngine.Vector3[] verts = new UnityEngine.Vector3[4];

                verts[0] = crossSectionRenderer2.GetPosition(0);
                verts[1] = crossSectionRenderer2.GetPosition(1);
                verts[2] =
                    (crossSectionRenderer3.GetPosition(1) == verts[0] ||
                    crossSectionRenderer3.GetPosition(1) == verts[1])
                        ? crossSectionRenderer3.GetPosition(0)
                        : crossSectionRenderer3.GetPosition(1);
                verts[3] =
                    (crossSectionRenderer4.GetPosition(1) == verts[0] ||
                    crossSectionRenderer4.GetPosition(1) == verts[1] ||
                    crossSectionRenderer4.GetPosition(1) == verts[2])
                        ? crossSectionRenderer4.GetPosition(0)
                        : crossSectionRenderer4.GetPosition(1);

                tetrahedronMesh.vertices = verts;

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
                tetrahedronMesh.triangles = tris;
            }
        }
    }
}
