namespace IMRE.ScaleDimension.CrossSections

{
    public class TriangleCrossSection : UnityEngine.MonoBehaviour
    {
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