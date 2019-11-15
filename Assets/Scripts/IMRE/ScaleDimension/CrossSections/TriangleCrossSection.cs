using Unity.Mathematics;
using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections

{
    public class TriangleCrossSection : UnityEngine.MonoBehaviour
    {
        public Unity.Mathematics.float3 lineDir;
        public Unity.Mathematics.float3 linePos;
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
            xc.startWidth = 0.1f;
            xc.endWidth = 0.1f;
            xc.loop = false;
        }

        private void Update()
        {
            crossSectTri(linePos, lineDir, new[] {triangleVerticies.c0, triangleVerticies.c1, triangleVerticies.c2},
                xc);
        }

        /// <summary>
        ///     Function to render the intersection of a plane and a triangle
        /// </summary>
        /// <param name="height"></param>
        /// <param name="vertices"></param>
        /// <param name="crossSectionRenderer"></param>
        public void crossSectTri(Unity.Mathematics.float3 point, Unity.Mathematics.float3 direction,
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
            Unity.Mathematics.float3 ac_star = intersectLines(point, direction, a, ac_hat);
            Unity.Mathematics.float3 ab_star = intersectLines(point, direction, c, ab_hat);
            Unity.Mathematics.float3 bc_star = intersectLines(point, direction, b, bc_hat);

           //boolean values for if intersection hits only a vertex of the triangle
            bool ac_star_isEndpoint;
            ac_star_isEndpoint = ac_star.Equals(a) || ac_star.Equals(c);
            bool ab_star_isEndpoint;
            ab_star_isEndpoint = ab_star.Equals(a) || ab_star.Equals(b);
            bool bc_star_isEndpoint;
            bc_star_isEndpoint = bc_star.Equals(c) || bc_star.Equals(c);

            //booleans for if intersection hits somewhere on the segments
            bool ac_star_onSegment =
                UnityEngine.Vector3.Magnitude(ac_star - a) < UnityEngine.Vector3.Magnitude(c - a) ||
                UnityEngine.Vector3.Magnitude(ac_star - c) < UnityEngine.Vector3.Magnitude(c - a);
            bool ab_star_onSegment =
                UnityEngine.Vector3.Magnitude(ab_star - a) < UnityEngine.Vector3.Magnitude(b - a) ||
                UnityEngine.Vector3.Magnitude(ab_star - c) < UnityEngine.Vector3.Magnitude(b - a);
            bool bc_star_onSegment =
                UnityEngine.Vector3.Magnitude(bc_star - b) < UnityEngine.Vector3.Magnitude(c - b) ||
                UnityEngine.Vector3.Magnitude(bc_star - c) < UnityEngine.Vector3.Magnitude(c - b);

            //track how many vertices the intersection hits
            int endpointCount = 0;
            if (ac_star_isEndpoint)
                endpointCount++;
            if (ab_star_isEndpoint)
                endpointCount++;
            if (bc_star_isEndpoint)
                endpointCount++;

            Unity.Mathematics.float3 a_ab_intersection = intersectLines(point, direction, a, ab_star);

            //If plane does not hit triangle
            if (!(ab_star_onSegment || ac_star_onSegment || bc_star_onSegment))
            {
                crossSectionRenderer.enabled = false;
                UnityEngine.Debug.Log("Line does not intersect with any of triangle sides.");
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
                    if (a_ab_intersection.Equals(point) ||
                        intersectLines(point, direction, a, ac_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    //b is trivial
                    else if (intersectLines(point, direction, b, ab_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    //c is trivial
                    else if (intersectLines(point, direction, c, ac_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, b);
                    }
                }
                else if (!ab_star.Equals(bc_star))
                {
                    if (a_ab_intersection.Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    else if (intersectLines(point, direction, b, ab_star).Equals(point) ||
                             intersectLines(point, direction, b, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    else if (intersectLines(point, direction, c, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, b);
                    }
                }

                if (!ac_star.Equals(bc_star))
                {
                    if (intersectLines(point, direction, a, ac_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, b);
                        crossSectionRenderer.SetPosition(1, c);
                    }
                    else if (intersectLines(point, direction, c, ac_star).Equals(point) ||
                             intersectLines(point, direction, c, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, b);
                    }
                    else if (intersectLines(point, direction, b, bc_star).Equals(point))
                    {
                        crossSectionRenderer.SetPosition(0, a);
                        crossSectionRenderer.SetPosition(1, c);
                    }
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
            }
            
        }

        /// <summary>
        ///     Returns the point of intersection of two lines
        /// </summary>
        /// <param name="p"></param>
        /// <param name="u"></param>
        /// <param name="q"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private Unity.Mathematics.float3 intersectLines(Unity.Mathematics.float3 p, Unity.Mathematics.float3 u,
            Unity.Mathematics.float3 q, Unity.Mathematics.float3 v)
        {

            return LineLineIntersection(p, q, u, v);
//            //TODO Audit This Function
//
//            //using method described here: http://geomalgorithms.com/a05-_intersect-1.html
//            Unity.Mathematics.float3 w = q - p;
//
//            //TODO check cross products
//            Unity.Mathematics.float3 v_perp =
//                Unity.Mathematics.math.normalize(Unity.Mathematics.math.cross(Unity.Mathematics.math.cross(u, v), v));
//            Unity.Mathematics.float3 u_perp =
//                Unity.Mathematics.math.normalize(Unity.Mathematics.math.cross(Unity.Mathematics.math.cross(v, u), u));
//            float s = Unity.Mathematics.math.dot(-1 * v_perp, w) / Unity.Mathematics.math.dot(v_perp, u);
//
//            //note if s == 0, lines are parallel
//            Unity.Mathematics.float3 solution = p + s * u;
//
//            //the next couple of lines don't calculate a solution but can validate our solution.
//            float t = Unity.Mathematics.math.dot(u_perp, w) / Unity.Mathematics.math.dot(u_perp, v);
//
//            //note that if t == 0, lines are parallel
//            Unity.Mathematics.float3 solution_alt = q + t * v;
//
//            if (!solution.Equals(solution_alt))
//                UnityEngine.Debug.LogWarning("Invalid Solution to Intersection of Lines");
//            return solution;
//
//            //return new Unity.Mathematics.float3(UnityEngine.Mathf.Infinity, UnityEngine.Mathf.Infinity,
//            //UnityEngine.Mathf.Infinity);
        }


        /// <summary>
        /// Finds the intersection between two lines
        /// </summary>
        /// <param name="linePos1">a point on the first line</param>
        /// <param name="linePos2">a point on the second line</param>
        /// <param name="lineDir1">the direction of the first line</param>
        /// <param name="lineDir2">the direction of the second line</param>
        /// <returns></returns>
        internal static Unity.Mathematics.float3 LineLineIntersection(Unity.Mathematics.float3 linePos1, Unity.Mathematics.float3 linePos2, Unity.Mathematics.float3 lineDir1,
            Unity.Mathematics.float3 lineDir2)
        {


            //check for skew lines and parallel lines.
            bool tBool1 = Vector3.Cross(lineDir1, lineDir2).magnitude > 0;
            bool tBool2 = Vector3.Project(linePos1 - linePos2, Vector3.Cross(lineDir1, lineDir2)).magnitude == 0;

            //UnityEngine.Debug.Log(Vector3.Cross(lineDir1, lineDir2).magnitude);
            //UnityEngine.Debug.Log(Vector3.Project(linePos1 - linePos2, Vector3.Cross(lineDir1, lineDir2)).magnitude);
            
            //if the two lines intersect, they will intersect at the intersection point of line1 and the plane of line1 and point 2 on the plane, where 
            if (tBool1 && tBool2)
            {
                return LinePlaneIntersection(linePos1, lineDir1, linePos2,
                    Vector3.Cross(Vector3.Cross(lineDir1, lineDir2), lineDir2));
            }
            else
            {
                UnityEngine.Debug.LogWarning("Lines do not intersect");
                return float3.zero;
            }
    }
        
        /// <summary>
        /// Finds the point of intersection between a line and a plane
        /// </summary>
        /// <param name="linePos">A point on the line</param>
        /// <param name="lineDir">The direction of the line</param>
        /// <param name="planePos">A point on the plane</param>
        /// <param name="planeNorm">The normal direction of the plane</param>
        /// <returns></returns>
        internal static Unity.Mathematics.float3 LinePlaneIntersection(Unity.Mathematics.float3 linePos, Unity.Mathematics.float3 lineDir, Unity.Mathematics.float3 planePos, Unity.Mathematics.float3 planeNorm)
        {
            //        //using Ians math.  Something isn't consistant.  Try using unity math instead.
            //        if (Vector3.ProjectOnPlane(lineDir,planeNorm).magnitude > 0)
            //        {
            //            Vector3 w = (linePos-planePos);
            //            Vector3 u = lineDir.normalized;
            //            Vector3 n = planeNorm.normalized;
            //Vector3 v_0 = planePos;
            //float s = -Vector3.Dot(n, w) / Vector3.Dot(n, u);
            //Vector3 pointPos = s * u + w + v_0;
            //            return new intersectionFigData { figtype = GeoObjType.point, vectordata = new Vector3[] { pointPos}};
            //        }
            //        else
            //        {
            //            return new intersectionFigData { figtype = GeoObjType.none };
            //        }

            UnityEngine.Plane p = new UnityEngine.Plane(planeNorm, planePos);
            UnityEngine.Ray l = new UnityEngine.Ray(linePos, Vector3.Normalize(lineDir));
            float dist = UnityEngine.Mathf.Infinity;
            p.Raycast(l, out dist);
            if(dist == UnityEngine.Mathf.Infinity)
            {
                l = new UnityEngine.Ray(linePos, - Vector3.Normalize(lineDir));
                p.Raycast(l, out dist);
            }
            if (dist == UnityEngine.Mathf.Infinity)
            {
                return float3.zero;
            }

            Vector3 linePosVec3 = new Vector3(linePos.x, linePos.y, linePos.z);
            
            Debug.Log(linePosVec3 + Vector3.Normalize(lineDir * dist));
            return linePosVec3 + Vector3.Normalize(lineDir * dist);


        }
    }
}