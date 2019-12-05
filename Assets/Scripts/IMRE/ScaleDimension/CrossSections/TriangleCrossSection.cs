using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections

{
    public class TriangleCrossSection : AbstractCrossSection
    {
        private UnityEngine.LineRenderer obj;
        public Unity.Mathematics.float3x3 triangleVerticies;

        private UnityEngine.LineRenderer xc;
        private void Start()
        {
            xc = gameObject.AddComponent<UnityEngine.LineRenderer>();
            //obj= gameObject.AddComponent<UnityEngine.LineRenderer>();
            //obj.SetPositions(new [] {(UnityEngine.Vector3) triangleVerticies.c0,(UnityEngine.Vector3) triangleVerticies.c1,(UnityEngine.Vector3) triangleVerticies.c2});
            //obj.startWidth = 0.1f;
            //obj.endWidth = 0.1f;
            //obj.loop = true;
            //obj.useWorldSpace = true;
            xc.useWorldSpace = true;
            xc.startWidth = 0.01f;
            xc.endWidth = 0.01f;
            xc.loop = false;

            xc.material = mat;
        }

        private void Update()
        {
            crossSectTri(planePos, planeNormal, new[] {triangleVerticies.c0, triangleVerticies.c1, triangleVerticies.c2},
                xc);
        }

        /// <summary>
        ///     Function to render the intersection of a plane and a triangle
        /// </summary>
        /// <param name="height"></param>
        /// <param name="vertices"></param>
        /// <param name="crossSectionRenderer"></param>
        public void crossSectTri(Unity.Mathematics.float3 point, Unity.Mathematics.float3 normalDirection,
            Unity.Mathematics.float3[] vertices, UnityEngine.LineRenderer crossSectionRenderer)
        {
            //Vertices are organized in clockwise manner starting from top
            //top vertex
            Unity.Mathematics.float3 a = vertices[0];
            //bottom right
            Unity.Mathematics.float3 b = vertices[1];
            //bottom left
            Unity.Mathematics.float3 c = vertices[2];

            //intermediate calculations 
            Unity.Mathematics.float3 ac_hat = (c - a) / UnityEngine.Vector3.Magnitude(c - a);
            Unity.Mathematics.float3 ab_hat = (b - a) / UnityEngine.Vector3.Magnitude(b - a);
            Unity.Mathematics.float3 bc_hat = (c - b) / UnityEngine.Vector3.Magnitude(c - b);
            
            //points of intersection on each line segment
            Unity.Mathematics.float3 ac_star = IMRE.Math.Operations.SegmentPlaneIntersection(a,c,point,normalDirection);
            Unity.Mathematics.float3 ab_star = IMRE.Math.Operations.SegmentPlaneIntersection(a, b,  point, normalDirection);
            Unity.Mathematics.float3 bc_star = IMRE.Math.Operations.SegmentPlaneIntersection(b, c, point, normalDirection);
            
            //boolean values for if intersection hits only a vertex of the triangle
            bool ac_star_isEndpoint;
            ac_star_isEndpoint = ac_star.Equals(a) || ac_star.Equals(c);
            bool ab_star_isEndpoint;
            ab_star_isEndpoint = ab_star.Equals(a) || ab_star.Equals(b);
            bool bc_star_isEndpoint;
            bc_star_isEndpoint = bc_star.Equals(c) || bc_star.Equals(c);

            //booleans for if intersection hits somewhere on the segments
            bool ac_star_onSegment = !ac_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            bool ab_star_onSegment = !ab_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            bool bc_star_onSegment = !bc_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));

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
            }

            //intersection is a segment (edge) of the triangle
            //the concept for choosing the right points of the cross section is the same for each of these subcases
            //two unique endpoint values
            else if (endpointCount >= 2 &&
                     (!ab_star.Equals(ac_star) || !ab_star.Equals(bc_star) || !ac_star.Equals(bc_star)))
            {
                crossSectionRenderer.enabled = true;
                //if there are two unique values, pick two.
                //drop the result for the edge which is within the plane
                
                //tolerance accounts for rounding errors
                float tolerance = .00001f;
                if (Unity.Mathematics.math.dot(ab_hat, normalDirection) < tolerance)
                {
                    crossSectionRenderer.SetPosition(0,a);
                    crossSectionRenderer.SetPosition(1,b);
                }
                else if (Unity.Mathematics.math.dot(ac_hat, normalDirection) < tolerance)
                {
                    crossSectionRenderer.SetPosition(0,a);
                    crossSectionRenderer.SetPosition(1,c);
                }
                else if (Unity.Mathematics.math.dot(bc_hat, normalDirection) < tolerance)
                {
                    crossSectionRenderer.SetPosition(0,b);
                    crossSectionRenderer.SetPosition(1,c);
                }
                else
                {
                    Debug.LogWarning("Error in calculation of line plane intersection");
                    crossSectionRenderer.enabled = false;
                }
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
                else
                {
                    Debug.LogWarning("Error in calculation of line plane intersection");
                    crossSectionRenderer.enabled = false;
                }
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
                else if (ab_star_onSegment && bc_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, ab_star);
                    crossSectionRenderer.SetPosition(1, bc_star);
                }
                else
                {
                   
                    Debug.LogWarning("Error in calculation of line plane intersection");
                    crossSectionRenderer.enabled = false;
                }
            }

            
            
        }
        
        
    }
}
