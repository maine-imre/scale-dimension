using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.Experimental.PlayerLoop;


namespace IMRE.ScaleDimension.CrossSections
{
    public class TetrahderonCrossSection : UnityEngine.MonoBehaviour
    {
        public void crossSectTetrahedron(float3 point, float3 normalDirection, float3[] vertices,
            LineRenderer tetrahedronRenderer, Mesh tetrahedronMesh)
        {
            //take vertices and organize them into clockwise triangles to be passed to triangle intersection function
            float3 a = vertices[0];
            float3 b = vertices[1];
            float3 c = vertices[2];
            float3 d = vertices[3];

            float3[] triangle1 = {a, b, c};
            float3[] triangle2 = {a, c, d};
            float3[] triangle3 = {a, d, b};
            float3[] triangle4 = {b, c, d};
            
            float3 triangle1Normal = math.cross(Vector3.Normalize(c - a), Vector3.Normalize(b - a));
            float3 triangle2Normal = math.cross(Vector3.Normalize(c - a), Vector3.Normalize(d - a));
            float3 triangle3Normal = math.cross(Vector3.Normalize(d - a), Vector3.Normalize(b - a));
            float3 triangle4Normal = math.cross(Vector3.Normalize(c - b), Vector3.Normalize(d - b));

            float3 testpoint = new float3();
            float3 testdirection = new float3();

            float3 tri1Point = intersectPlanes(testpoint, testdirection, triangle1Normal, a)[0];
            float3 tri1Dir = intersectPlanes(testpoint, testdirection, triangle1Normal, a)[1];

            float3 tri2Point = intersectPlanes(testpoint, testdirection, triangle2Normal, a)[0];
            float3 tri2Dir = intersectPlanes(testpoint, testdirection, triangle2Normal, a)[1];

            float3 tri3Point = intersectPlanes(testpoint, testdirection, triangle3Normal, a)[0];
            float3 tri3Dir = intersectPlanes(testpoint, testdirection, triangle3Normal, a)[1];

            float3 tri4Point = intersectPlanes(testpoint, testdirection, triangle4Normal, b)[0];
            float3 tri4Dir = intersectPlanes(testpoint, testdirection, triangle4Normal, b)[1];

            LineRenderer crossSectionRenderer1 = crossSectTri(tri1Point, tri1Dir, triangle1, tetrahedronRenderer);
            LineRenderer crossSectionRenderer2 = crossSectTri(tri2Point, tri2Dir, triangle2, tetrahedronRenderer);
            LineRenderer crossSectionRenderer3 = crossSectTri(tri3Point, tri3Dir, triangle3, tetrahedronRenderer);
            LineRenderer crossSectionRenderer4 = crossSectTri(tri4Point, tri4Dir, triangle4, tetrahedronRenderer);

            int vertCount = 0;

            bool line1 = false, line2 = false, line3 = false, line4 = false;

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
            
           //Intersection is Triangle
           if(vertCount == 3)
           {
               //verts
               Vector3[] verts = new Vector3[3];

               if (line1 && line2 && line3)
               {
                   
                   verts[0] = crossSectionRenderer1.GetPosition(0);
                   verts[1] = crossSectionRenderer2.GetPosition(0);
                   verts[2] = crossSectionRenderer3.GetPosition(0);
               }
               
               else if (line1 && line2 && line4)
               {
                   
                   verts[0] = crossSectionRenderer1.GetPosition(0);
                   verts[1] = crossSectionRenderer2.GetPosition(0);
                   verts[2] = crossSectionRenderer4.GetPosition(0);
               }
               
               else if (line1 && line3 && line4)
               {
                   
                   verts[0] = crossSectionRenderer1.GetPosition(0);
                   verts[1] = crossSectionRenderer3.GetPosition(0);
                   verts[2] = crossSectionRenderer4.GetPosition(0);
               }
               
               else if (line2 && line3 && line4)
               {
                   
                   verts[0] = crossSectionRenderer2.GetPosition(0);
                   verts[1] = crossSectionRenderer3.GetPosition(0);
                   verts[2] = crossSectionRenderer4.GetPosition(0);
               }
               
               tetrahedronMesh.vertices = verts;

               //tris
               int[] tris = {0, 1, 2};
               tetrahedronMesh.triangles = tris;
            }
           //Intersection is quadrilateral
            else if(vertCount == 4)
            {
                //verts
                Vector3[] verts = new Vector3[4];
                verts[0] = crossSectionRenderer1.GetPosition(0);
                verts[1] = crossSectionRenderer2.GetPosition(0);
                verts[2] = crossSectionRenderer3.GetPosition(0);
                verts[3] = crossSectionRenderer4.GetPosition(0);

                tetrahedronMesh.vertices = verts;

                //tris
                int[] tris = {0, 1, 3, 1, 2, 3};
                tetrahedronMesh.triangles = tris;
            }

        }

        //TODO: check that this function is correct - also make sure that case where planes do not intersect is met, i.e. 
        /// <summary>
        /// function that calculates intersection of two planes, returns a line segment
        /// using mathematics described here http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <returns></returns>
        public float3[] intersectPlanes(float3 plane1Point, float3 normal1, float3 plane2Point, float3 normal2)
        {
            float3 u = Vector3.Normalize(math.cross(normal1, normal2));

            float3 lineDirInPlane2 = Vector3.Normalize(math.cross(normal2, u));

            //TODO check that the dot product is used correctly for projection
            float3 lineDirInPlane2_prime = lineDirInPlane2 - math.dot(normal1, lineDirInPlane2) * normal1;

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
        
        private UnityEngine.LineRenderer crossSectTri(Unity.Mathematics.float3 point, Unity.Mathematics.float3 direction,
            Unity.Mathematics.float3[] vertices, UnityEngine.LineRenderer crossSectionRenderer)
        {
            //Vertices are organized in clockwise manner starting from top
            //top vertex
            Unity.Mathematics.float3 a = vertices[0];
            //bottom right
            Unity.Mathematics.float3 b = vertices[1];
            //bottom left
            Unity.Mathematics.float3 c = vertices[2];

            Unity.Mathematics.float3 lineDirection = direction - point;

            //intermediate calculations 
            Unity.Mathematics.float3 ac_hat = (c - a) / UnityEngine.Vector3.Magnitude(c - a);
            Unity.Mathematics.float3 ab_hat = (b - a) / UnityEngine.Vector3.Magnitude(b - a);
            Unity.Mathematics.float3 bc_hat = (c - b) / UnityEngine.Vector3.Magnitude(c - b);

            //points of intersection on each line segment
            Unity.Mathematics.float3 ac_star = intersectLines(point, lineDirection, a, ac_hat);
            Unity.Mathematics.float3 ab_star = intersectLines(point, lineDirection, a, ab_hat);
            Unity.Mathematics.float3 bc_star = intersectLines(point, lineDirection, b, bc_hat);

            //boolean values for if intersection hits only a vertex of the triangle
            bool ac_star_isEndpoint;
            ac_star_isEndpoint = ac_star.Equals(a) || ac_star.Equals(c);
            bool ab_star_isEndpoint;
            ab_star_isEndpoint = ab_star.Equals(a) || ab_star.Equals(b);
            bool bc_star_isEndpoint;
            bc_star_isEndpoint = bc_star.Equals(c) || bc_star.Equals(c);

            //booleans for if intersection hits somewhere on the segments
            bool ac_star_onSegment =
                UnityEngine.Vector3.Magnitude(ac_star - a) > UnityEngine.Vector3.Magnitude(c - a) ||
                UnityEngine.Vector3.Magnitude(ac_star - c) > UnityEngine.Vector3.Magnitude(c - a);
            bool ab_star_onSegment =
                UnityEngine.Vector3.Magnitude(ab_star - a) > UnityEngine.Vector3.Magnitude(b - a) ||
                UnityEngine.Vector3.Magnitude(ab_star - c) > UnityEngine.Vector3.Magnitude(b - a);
            bool bc_star_onSegment =
                UnityEngine.Vector3.Magnitude(bc_star - b) > UnityEngine.Vector3.Magnitude(c - b) ||
                UnityEngine.Vector3.Magnitude(bc_star - c) > UnityEngine.Vector3.Magnitude(c - b);

            //track how many vertices the intersection hits
            int endpointCount = 0;
            if (ac_star_isEndpoint)
                endpointCount++;
            if (ab_star_isEndpoint)
                endpointCount++;
            if (bc_star_isEndpoint)
                endpointCount++;

            //If plane does not hit triangle
            if (!(ab_star_onSegment || ac_star_onSegment || bc_star_onSegment))
            {
                crossSectionRenderer.enabled = false;
                UnityEngine.Debug.Log("Line does not intersect with any of triangle sides.");

                return crossSectionRenderer;
            }

            //intersection is a segment (edge) of the triangle
            //the concept for choosing the right points of the cross section is the same for each of these subcases
            else if (endpointCount >= 2 &&
                     (!ab_star.Equals(ac_star) || !ab_star.Equals(bc_star) || !ac_star.Equals(bc_star)))
            {
                crossSectionRenderer.enabled = true;
                //ab_star and ac_star both do not equal a
                if (!ab_star.Equals(ac_star))
                {
                    //find out which case is trivial and use the other two vertices; if it is trivial the intersection will just be the value passed in to the function
                    //a is trivial
                    if (intersectLines(point, lineDirection, a, ab_star).Equals(point) ||
                        intersectLines(point, lineDirection, a, ac_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    //b is trivial
                    else if (intersectLines(point, lineDirection, b, ab_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    //c is trivial
                    else if (intersectLines(point, lineDirection, c, ac_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, b);
                    }

                }
                else if (!ab_star.Equals(bc_star))
                {
                    if (intersectLines(point, lineDirection, a, ab_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    else if (intersectLines(point, lineDirection, b, ab_star).Equals(point) ||
                             intersectLines(point, lineDirection, b, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    else if (intersectLines(point, lineDirection, c, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, b);
                    }

                }

                if (!ac_star.Equals(bc_star))
                {
                    if (intersectLines(point, lineDirection, a, ac_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    else if (intersectLines(point, lineDirection, c, ac_star).Equals(point) ||
                             intersectLines(point, lineDirection, c, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, b);
                    }
                    else if (intersectLines(point, lineDirection, b, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                }

                return crossSectionRenderer;
            }

            //intersection hits one vertex on triangle and one of the segments
            else if (endpointCount == 2 && ab_star.Equals(ac_star) || ab_star.Equals(bc_star) ||
                     ac_star.Equals(bc_star))
            {
                crossSectionRenderer.enabled = true;

                //if point of intersection hits a, it must hit bc_star; same logic applies to remaining subcases
                if (ab_star.Equals(ac_star))
                {
                    crossSectionRenderer.SetPosition(0, a);
                    crossSectionRenderer.SetPosition(1, bc_star);
                }
                else if (ab_star.Equals(bc_star))
                {
                    crossSectionRenderer.SetPosition(0, b);
                    crossSectionRenderer.SetPosition(1, ac_star);
                }
                else if (ac_star.Equals(bc_star))
                {
                    crossSectionRenderer.SetPosition(0, c);
                    crossSectionRenderer.SetPosition(1, ab_star);
                }

                return crossSectionRenderer;
            }
            //intersection hits somewhere on two different segments of triangle; last remaining case
            else
            {
                crossSectionRenderer.enabled = true;

                //find out which two segments are intersected and use their calculated intersections
                if (ac_star_onSegment && ab_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, ac_star);
                    crossSectionRenderer.SetPosition(1, ab_star);
                }
                else if (ac_star_onSegment && bc_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, ac_star);
                    crossSectionRenderer.SetPosition(1, bc_star);
                }
                else
                {
                    crossSectionRenderer.SetPosition(0, ab_star);
                    crossSectionRenderer.SetPosition(1, bc_star);
                }
                
                return crossSectionRenderer;
            }
        }
        
        private Unity.Mathematics.float3 intersectLines(Unity.Mathematics.float3 p, Unity.Mathematics.float3 u,
            Unity.Mathematics.float3 q, Unity.Mathematics.float3 v)
        {
            //using method described here: http://geomalgorithms.com/a05-_intersect-1.html
            Unity.Mathematics.float3 w = q - p;
            Unity.Mathematics.float3 v_perp =
                Unity.Mathematics.math.normalize(Unity.Mathematics.math.cross(Unity.Mathematics.math.cross(u, v), v));
            Unity.Mathematics.float3 u_perp =
                Unity.Mathematics.math.normalize(Unity.Mathematics.math.cross(Unity.Mathematics.math.cross(u, v), u));
            float s = Unity.Mathematics.math.dot(-1 * v_perp, w) / Unity.Mathematics.math.dot(-1 * v_perp, u);

            //note if s == 0, lines are parallel
            Unity.Mathematics.float3 solution = p + s * u;

            //the next couple of lines don't calculate a solution but can validate our solution.
            float t = Unity.Mathematics.math.dot(-1 * u_perp, w) / Unity.Mathematics.math.dot(-1 * u_perp, v);

            //note that if t == 0, lines are parallel
            Unity.Mathematics.float3 solution_alt = q + t * v;

            if (solution.Equals(solution_alt)) return solution;
            
            UnityEngine.Debug.LogWarning("Invalid Solution to Intersection of Lines");
            
            return new Unity.Mathematics.float3(UnityEngine.Mathf.Infinity, UnityEngine.Mathf.Infinity,
                UnityEngine.Mathf.Infinity);
        }

    }
}