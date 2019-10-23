namespace IMRE.ScaleDimension.CrossSections
{
    public class SquareCrossSection : UnityEngine.MonoBehaviour
    {
        /// <summary>
        ///     Function to render the intersection of a plane and a square
        /// </summary>
        /// <param name="height"></param>
        /// <param name="vertices"></param>
        /// <param name="crossSectionRenderer"></param>
        public void crossSectSquare(Unity.Mathematics.float3 point, Unity.Mathematics.float3 direction,
            UnityEngine.Vector3[] vertices,
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

            Unity.Mathematics.float3 lineDirection = direction - point;

            //intermediate calculations
            Unity.Mathematics.float3 ab_hat = (b - a) / UnityEngine.Vector3.Magnitude(b - a);
            Unity.Mathematics.float3 bc_hat = (c - b) / UnityEngine.Vector3.Magnitude(c - b);
            Unity.Mathematics.float3 cd_hat = (d - c) / UnityEngine.Vector3.Magnitude(d - c);
            Unity.Mathematics.float3 da_hat = (a - d) / UnityEngine.Vector3.Magnitude(a - d);

            //calculations for point of intersection on different segments
            Unity.Mathematics.float3 ab_star = intersectLines(point, lineDirection, a, ab_hat);
            Unity.Mathematics.float3 bc_star = intersectLines(point, lineDirection, b, bc_hat);
            Unity.Mathematics.float3 cd_star = intersectLines(point, lineDirection, c, cd_hat);
            Unity.Mathematics.float3 da_star = intersectLines(point, lineDirection, d, da_hat);

            //booleans for if the intersection hits a vertex 
            bool ab_star_isEndpoint = ab_star.Equals(a) || ab_star.Equals(b);
            bool bc_star_isEndpoint = bc_star.Equals(b) || bc_star.Equals(c);
            bool cd_star_isEndpoint = cd_star.Equals(c) || cd_star.Equals(d);
            bool da_star_isEndpoint = da_star.Equals(d) || da_star.Equals(a);

            //booleans for if the intersection hits somewhere on the segments besides the vertices
            bool ab_star_onSegment =
                UnityEngine.Vector3.Magnitude(ab_star - a) > UnityEngine.Vector3.Magnitude(b - a) ||
                UnityEngine.Vector3.Magnitude(ab_star - b) > UnityEngine.Vector3.Magnitude(b - a);
            bool bc_star_onSegment =
                UnityEngine.Vector3.Magnitude(bc_star - b) > UnityEngine.Vector3.Magnitude(c - b) ||
                UnityEngine.Vector3.Magnitude(bc_star - c) > UnityEngine.Vector3.Magnitude(c - b);
            bool cd_star_onSegment =
                UnityEngine.Vector3.Magnitude(cd_star - c) > UnityEngine.Vector3.Magnitude(d - c) ||
                UnityEngine.Vector3.Magnitude(cd_star - d) > UnityEngine.Vector3.Magnitude(d - c);
            bool da_star_onSegment =
                UnityEngine.Vector3.Magnitude(da_star - d) > UnityEngine.Vector3.Magnitude(a - d) ||
                UnityEngine.Vector3.Magnitude(da_star - a) > UnityEngine.Vector3.Magnitude(a - d);

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
                }

                crossSectionRenderer.SetPosition(0, result0);
                crossSectionRenderer.SetPosition(1, result1);
            }

            //intersection hits one vertice and somewhere on a segment
            if (endpointCount == 2 && (ab_star.Equals(bc_star) || bc_star.Equals(cd_star) || cd_star.Equals(da_star)))
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
                else
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
                else
                {
                    crossSectionRenderer.SetPosition(0, cd_star);
                    crossSectionRenderer.SetPosition(1, da_star);
                }
            }
        }

        //these are simply copy-pasted for now

        #region functions

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

        #endregion
    }
}