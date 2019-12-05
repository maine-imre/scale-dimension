using UnityEngine;
using Unity.Mathematics;

namespace IMRE.ScaleDimension.CrossSections
{
    public class SquareCrossSection : AbstractCrossSection
    {
        private UnityEngine.LineRenderer obj;
        public Unity.Mathematics.float3x4 squareVertices;

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
            crossSectSquare(planePos, planeNormal,
                new[] {squareVertices.c0, squareVertices.c1, squareVertices.c2, squareVertices.c3},
                xc);
        }

        /// <summary>
        /// Function to render the intersection of a plane and a square
        /// </summary>
        /// <param name="height"></param>
        /// <param name="vertices"></param>
        /// <param name="crossSectionRenderer"></param>
        public void crossSectSquare(Unity.Mathematics.float3 point, Unity.Mathematics.float3 normalDirection,
            float3[] vertices,
            UnityEngine.LineRenderer crossSectionRenderer)
        {
            //Vertices are organized in clockwise manner starting from top left
            //top left
            Unity.Mathematics.float3 a = vertices[0];
            //top right
            Unity.Mathematics.float3 b = vertices[1];
            //bottom right
            Unity.Mathematics.float3 c = vertices[2];
            //bottom left
            Unity.Mathematics.float3 d = vertices[3];

            //intermediate calculations
            Unity.Mathematics.float3 ab_hat = (b - a) / UnityEngine.Vector3.Magnitude(b - a);
            Unity.Mathematics.float3 bc_hat = (c - b) / UnityEngine.Vector3.Magnitude(c - b);
            Unity.Mathematics.float3 cd_hat = (d - c) / UnityEngine.Vector3.Magnitude(d - c);
            Unity.Mathematics.float3 da_hat = (a - d) / UnityEngine.Vector3.Magnitude(a - d);

            //calculations for point of intersection on different segments
            Unity.Mathematics.float3 ab_star = IMRE.Math.Operations.SegmentPlaneIntersection(a, b, point, normalDirection);
            Unity.Mathematics.float3 bc_star = IMRE.Math.Operations.SegmentPlaneIntersection(b, c, point, normalDirection);
            Unity.Mathematics.float3 cd_star = IMRE.Math.Operations.SegmentPlaneIntersection(c, d, point, normalDirection);
            Unity.Mathematics.float3 da_star = IMRE.Math.Operations.SegmentPlaneIntersection(d, a, point, normalDirection);

            //booleans for if the intersection hits a vertex 
            bool ab_star_isEndpoint = ab_star.Equals(a) || ab_star.Equals(b);
            bool bc_star_isEndpoint = bc_star.Equals(b) || bc_star.Equals(c);
            bool cd_star_isEndpoint = cd_star.Equals(c) || cd_star.Equals(d);
            bool da_star_isEndpoint = da_star.Equals(d) || da_star.Equals(a);

            //booleans for if the intersection hits somewhere on the segments besides the vertices
            bool ab_star_onSegment =
                !ab_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            bool bc_star_onSegment =
                !bc_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            bool cd_star_onSegment =
                !cd_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            bool da_star_onSegment =
                !da_star.Equals(new float3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));

            Debug.Log(ab_star);
            Debug.Log(bc_star);
            Debug.Log(cd_star);
            Debug.Log(da_star);

            //track how many vertices are hit in the intersection
            int endpointCount = 0;
            if (ab_star_isEndpoint)
                endpointCount++;
            if (bc_star_isEndpoint)
                endpointCount++;
            if (cd_star_isEndpoint)
                endpointCount++;
            if (da_star_isEndpoint)
                endpointCount++;

            //intersection does not hit triangle
            if (!(ab_star_onSegment || bc_star_onSegment || cd_star_onSegment || da_star_onSegment))
            {
                crossSectionRenderer.enabled = false;
                UnityEngine.Debug.Log("Line does not intersect with any of triangle sides.");
            }
            //intersection is an edge of the square
            else if (endpointCount >= 2 &&
                     (!ab_star.Equals(bc_star) || !ab_star.Equals(da_star) || !bc_star.Equals(cd_star) ||
                      !cd_star.Equals(da_star)))
            {
                crossSectionRenderer.enabled = true;

                //Case where edge is the intersection

                //drop the two trivial cases, keep the two non-trivial cases.
                UnityEngine.Vector3 result0 = ab_star;
                UnityEngine.Vector3 result1 = cd_star;
                if (result0.Equals(point) || result1.Equals(point))
                {
                    //I know that the trivial cases are on opposite sides, because this is the case where the edge is the intersection.
                    //there are only two pairs of opposite sides.  if it's not one, it's the other.
                    result0 = bc_star;
                    result1 = da_star;

                    crossSectionRenderer.SetPosition(0, result0);
                    crossSectionRenderer.SetPosition(1, result1);
                }
                else
                {
                    Debug.LogWarning("Error in calculation of line plane intersection");
                    crossSectionRenderer.enabled = false;
                }

            }
            //intersection hits one vertice and somewhere on a segment
            else if (endpointCount == 2 &&
                     (ab_star.Equals(bc_star) || bc_star.Equals(cd_star) || cd_star.Equals(da_star)))
            {
                //find which vertex is in the intersection, and from there find which of the two possible segments are the other point of intersection
                //the same logic carries through all of these subcases
                if (ab_star.Equals(bc_star))
                {
                    if (cd_star_onSegment)
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, cd_star);
                    }
                    else
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, da_star);
                    }
                }
                else if (bc_star.Equals(cd_star))
                {
                    if (ab_star_onSegment)
                    {
                        crossSectionRenderer.SetPosition(0, c);
                        crossSectionRenderer.SetPosition(1, ab_star);
                    }
                    else
                    {
                        crossSectionRenderer.SetPosition(0, c);
                        crossSectionRenderer.SetPosition(1, da_star);
                    }
                }
                else if (cd_star.Equals(da_star))
                {
                    if (ab_star_onSegment)
                    {
                        crossSectionRenderer.SetPosition(0, d);
                        crossSectionRenderer.SetPosition(1, ab_star);
                    }
                    else
                    {
                        crossSectionRenderer.SetPosition(0, c);
                        crossSectionRenderer.SetPosition(1, bc_star);
                    }
                }
                else if (da_star.Equals(ab_star))
                {
                    if (cd_star_onSegment)
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, cd_star);
                    }

                    if (bc_star_onSegment)
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, bc_star);
                    }
                }
                else
                {
                    Debug.LogWarning("Error in calculation of line plane intersection");
                    crossSectionRenderer.enabled = false;
                }
            }
            //intersection hits two segments of the square
            else
            {
                crossSectionRenderer.enabled = true;
                //use booleans to determine which two segments are in the intersection
                if (ab_star_onSegment && bc_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, ab_star);
                    crossSectionRenderer.SetPosition(1, bc_star);
                }
                else if (ab_star_onSegment && cd_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, ab_star);
                    crossSectionRenderer.SetPosition(1, cd_star);
                }
                else if (ab_star_onSegment && da_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, ab_star);
                    crossSectionRenderer.SetPosition(1, da_star);
                }
                else if (bc_star_onSegment && da_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, bc_star);
                    crossSectionRenderer.SetPosition(1, da_star);
                }
                else if (bc_star_onSegment && cd_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, bc_star);
                    crossSectionRenderer.SetPosition(1, cd_star);
                }
                else if (cd_star_onSegment && da_star_onSegment)
                {
                    crossSectionRenderer.SetPosition(0, cd_star);
                    crossSectionRenderer.SetPosition(1, da_star);
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
