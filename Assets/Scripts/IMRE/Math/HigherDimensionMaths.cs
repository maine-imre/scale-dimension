namespace IMRE.Math
{
    public enum ProjectionMethod
    {
        projective,
        parallel,
        stereographic
    }

    /// <summary>
    ///     Maths for 4D to 3D projection.
    /// </summary>
    public static class HigherDimensionsMaths
    {
        /// <summary>
        ///     Rotate the vector v around a plane for a given angle, in degrees.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="basis0"></param>
        /// <param name="basis1"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static UnityEngine.Vector4 rotate(this UnityEngine.Vector4 v, UnityEngine.Vector4 basis0,
            UnityEngine.Vector4 basis1, float theta)
        {
            basis0 = basis0.normalized;
            basis1 = basis1.normalized;
            UnityEngine.Vector4 remainder =
                v - (UnityEngine.Vector4.Project(v, basis0) + UnityEngine.Vector4.Project(v, basis1));
            theta *= UnityEngine.Mathf.Deg2Rad;

            if (UnityEngine.Vector3.Dot(basis0, basis1) != 0f)
                UnityEngine.Debug.LogWarning("Basis is not orthagonal");
            else if (UnityEngine.Vector4.Dot(v.normalized, basis0) != 1f || UnityEngine.Vector4.Dot(v, basis1) != 0f
            ) UnityEngine.Debug.LogWarning("Original Vector does not lie in the same plane as the first basis vector.");

            return UnityEngine.Vector4.Dot(v, basis0) *
                   (UnityEngine.Mathf.Cos(theta) * basis0 + basis1 * UnityEngine.Mathf.Sin(theta)) +
                   UnityEngine.Vector4.Dot(v, basis1) *
                   (UnityEngine.Mathf.Cos(theta) * basis1 + UnityEngine.Mathf.Sin(theta) * basis0) + remainder;
        }

        /// <summary>
        ///     Projects a vector onto another vector using a dot product.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static Unity.Mathematics.float4 project(this Unity.Mathematics.float4 v, Unity.Mathematics.float4 axis)
        {
            Unity.Mathematics.math.normalize(axis);
            return Unity.Mathematics.math.dot(v, axis) * axis;
        }

        /// <summary>
        ///     Return the basis of a hyper plane orthagonal to a given vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Unity.Mathematics.float4x3 basisSystem(this Unity.Mathematics.float4 v)
        {
            Unity.Mathematics.math.normalize(v);
            //use method described here:  https://www.geometrictools.com/Documentation/OrthonormalSets.pdf
            if (v.x == 0 && v.y == 0 && v.z == 0 && v.w == 0)
                UnityEngine.Debug.LogError("Can't form basis from zero vector");

            //the vector is the first basis vector for the 4-space, orthag to the hyperplane
            Unity.Mathematics.math.normalize(v);
            //establish a second basis vector
            Unity.Mathematics.float4 basis0;
            if (v.x != 0 || v.y != 0)
                basis0 = new UnityEngine.Vector4(v.y, v.x, 0, 0);
            else
                basis0 = new UnityEngine.Vector4(0, 0, v.w, v.z);

            Unity.Mathematics.math.normalize(basis0);

            float[] determinants = Determinant2X2(v, basis0);

            //index of largest determinant
            int idx = 0;
            for (int i = 0; i < 6; i++)
            {
                if (determinants[i] > determinants[idx])
                    idx = i;
            }

            if (determinants[idx] != 0) UnityEngine.Debug.LogError("No non-zero determinant");

            //choose bottom row of det matrix to generate next basis vector
            Unity.Mathematics.float4 bottomRow;
            if (idx == 0 || idx == 1 || idx == 3)
                bottomRow = new Unity.Mathematics.float4(0, 0, 0, 1);
            else if (idx == 2 || idx == 4)
                bottomRow = new Unity.Mathematics.float4(0, 0, 1, 0);
            else
                bottomRow = new Unity.Mathematics.float4(0, 1, 0, 0);

            Unity.Mathematics.float4 basis1 = determinantCoef(new Unity.Mathematics.float4x3(v, basis0, bottomRow));
            Unity.Mathematics.math.normalize(basis1);

            Unity.Mathematics.float4 basis2 = determinantCoef(new Unity.Mathematics.float4x3(v, basis0, basis1));
            Unity.Mathematics.math.normalize(basis2);

            //returns the basis that spans the hyperplane orthogonal to v
            Unity.Mathematics.float4x3 basis = new Unity.Mathematics.float4x3(basis0, basis1, basis2);
            //check that v is orthogonal.
//            v.projectDownDimension(basis, ProjectionMethod.parallel, null, null, null);
//            if (v.x != 0 || v.y != 0 || v.z != 0) UnityEngine.Debug.LogError("Basis is not orthogonal to v");

            return basis;
        }

        private static float[] Determinant2X2(Unity.Mathematics.float4 v0, Unity.Mathematics.float4 v1)
        {
            //find largest determinant of 2x2
            float[] determinants = new float[6];
            determinants[0] = v0.x * v1.y - v0.y * v1.x;
            determinants[1] = v0.x * v1.z - v0.z * v1.x;
            determinants[2] = v0.x * v1.w - v0.w * v1.x;
            determinants[3] = v0.y * v1.z - v0.z * v1.y;
            determinants[4] = v0.y * v1.w - v0.w * v1.y;
            determinants[5] = v0.z * v1.w - v0.w * v1.z;
            return determinants;
        }

        /// <summary>
        ///     Assume the following structure, return the determinant coeficients for v0, v1, v2, v3
        ///     v0 v1 v2 v3
        ///     x00 x01 x02 x03
        ///     x10 x11 x12 x13
        ///     x20 x21 x22 x23
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static Unity.Mathematics.float4 determinantCoef(Unity.Mathematics.float4x3 matrix)
        {
            Unity.Mathematics.float4 bottomRow = matrix.c2;
            float[] determinants = Determinant2X2(matrix.c0, matrix.c1);
            return new Unity.Mathematics.float4(
                bottomRow.y * determinants[5] - bottomRow.z * determinants[4] + bottomRow.w * determinants[3],
                -(bottomRow.x * determinants[5] - bottomRow.z * determinants[2] +
                  bottomRow.w * determinants[3]),
                bottomRow.x * determinants[4] - bottomRow.y * determinants[2] + bottomRow.w * determinants[0],
                -(bottomRow.x * determinants[3] - bottomRow.y * determinants[1] + bottomRow.z * determinants[0])
            );
        }
    }
}