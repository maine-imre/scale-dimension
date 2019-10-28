using System.Linq;

namespace IMRE.HandWaver.ScaleDimension
{
    public static class MeshOperations
    {
        public static UnityEngine.Mesh projectedMesh(Unity.Mathematics.float4[] verts, int[] tris)
        {
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            System.Collections.Generic.List<UnityEngine.Vector3> verticies =
                new System.Collections.Generic.List<UnityEngine.Vector3>();
            System.Collections.Generic.List<int> triangles = new System.Collections.Generic.List<int>();
            //Vector2[] uvs;
            //Vector3[] normals;
            //iterate through each face of the mesh

            for (int i = 0; i < tris.Length / 3; i++)
            {
                Unity.Mathematics.float4[] inputVerts =
                    {verts[tris[i * 3]], verts[tris[i * 3 + 1]], verts[tris[i * 3 + 2]]};
                Unity.Mathematics.float3[] tmpVerts;
                int[] tmpTris;
                projectTriangle(inputVerts, IMRE.ScaleDimension.SpencerStudyControl.ins.subdiv,
                    out tmpVerts, out tmpTris, IMRE.ScaleDimension.SpencerStudyControl.ins.projectionMethod);
                //stich faces into common mesh

                tmpTris.ToList().ForEach(idx => triangles.Add(idx + verticies.Count));
                tmpVerts.ToList().ForEach(vert => verticies.Add(vert));

                //TODO handle uvs;
                //TODO handle normals;
            }

            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();
            //mesh.uv = uvs;
            //mesh.normals = normals;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Typecasting
        /// </summary>
        /// <param name="vList"></param>
        /// <param name="dest"></param>
        /// <param name="start"></param>
        private static void CastArrayCopyTo(this Unity.Mathematics.float3[] vList, ref UnityEngine.Vector3[] dest,
            int start)
        {
            for (int i = 0; i < vList.Length; i++)
            {
                dest[i + start] = vList[i];
            }
        }

        public static void projectTriangle(Unity.Mathematics.float4[] verts, int subdiv,
            out Unity.Mathematics.float3[] out_verts, out int[] out_tris,
            IMRE.Math.ProjectionMethod method)
        {
            //break triangle into smaller triangles
            //use a modified Sierpiński triangle as a division mechanic (partition every triangle into 3 triangles each round). Then there are always 3^n triangles
            Unity.Mathematics.float4[] tmp_verts = verts;
            out_tris = new[] {0, 1, 2};
            //loop subdiv times on subdivide verts.
            //subdivide verts calls itself, but should terminate when it reaches a case where the size of a partition is 3.
            /*
             * Case 1 - triangle of 3 verticies - split into 6 (hit else case)
             * Case 2 - 6 verts - split in half (two of 3).  round 2, hit else case.
             * Case 3 - 12 verts - split in half (two of 6). round 2, split in half (four of 3), round 3 hit else case.
             * continue until we hit the subdiv'th case.
             */
            for (int i = 0; i < subdiv; i++) subdivideVerts(tmp_verts, out_tris, out tmp_verts, out out_tris);
            out_verts = new Unity.Mathematics.float3[tmp_verts.Length];

            for (int i = 0; i < tmp_verts.Length; i++)
            {
                out_verts[i] = projectPosition(tmp_verts[i]);
            }
        }

        private static void subdivideVerts(Unity.Mathematics.float4[] verts, int[] tris,
            out Unity.Mathematics.float4[] out_verts, out int[] out_tris)
        {
            //recursive case.
            out_verts = new Unity.Mathematics.float4[verts.Length == 3 ? 6 : verts.Length * 4];
            out_tris = new int[tris.Length * 4];
            if (verts.Length > 3)
            {
                Unity.Mathematics.float4[] tmpVerts;
                int[] tempTris;
                subdivideVerts(verts.GetRange(0, verts.Length / 2), tris, out tmpVerts, out tempTris);
                tmpVerts.CopyToRange(ref out_verts, 0, verts.Length - 1);
                tempTris.CopyTo(out_tris, 0);

                //symmetric case
                subdivideVerts(verts.GetRange(verts.Length / 2, verts.Length),
                    tris, out tmpVerts, out tempTris);
                UnityEngine.Debug.Log(out_tris.Length + " : " + tmpVerts.Length);
                tmpVerts.CopyToRange(ref out_verts, verts.Length, verts.Length * 2 - 1);
                //tempTris.CopyTo(out_tris, tempTris.Length);
                for (int i = 0; i < tmpVerts.Length; i++)
                {
                    out_tris[i + tempTris.Length] = tempTris[i];
                }
            }
            else
            {
                if (verts.Length != 3)
                    UnityEngine.Debug.LogWarning("ARRAY LENGTH SHOULD BE 3" + verts.Length);
                //assume that verts.Length = 3
                out_verts[0] = verts[0];
                out_verts[1] = verts[1];
                out_verts[2] = verts[2];
                out_verts[3] = (verts[0] + verts[1]) / 2f;
                out_verts[4] = (verts[1] + verts[2]) / 2f;
                out_verts[5] = (verts[2] + verts[0]) / 2f;

                out_tris = new[] {0, 3, 5, 1, 3, 4, 3, 4, 5, 4, 5, 2};
            }
        }

        private static Unity.Mathematics.float4[] GetRange(this Unity.Mathematics.float4[] v, int start, int end)
        {
            Unity.Mathematics.float4[] result = new Unity.Mathematics.float4[end - start];
            int length = System.Math.Min(v.Length, end - start);
            for (int i = 0; i < length; i++) result[i] = v[i + start];
            return result;
        }

        private static void CopyToRange(this Unity.Mathematics.float4[] v, ref Unity.Mathematics.float4[] dest,
            int start, int end)
        {
            int length = System.Math.Min(v.Length, end - start);
            for (int i = 0; i < length; i++) dest[i + start] = v[i];
        }

        public static Unity.Mathematics.float3 stereographicProjection(Unity.Mathematics.float4 pos)
        {
            //then inflate each vert to the edge of the sphere and keep that value.
            pos = pos / IMRE.Math.Operations.magnitude(pos);

            //TODO turn to make north pole float4.right
//            IMRE.Math.HigherDimensionsMaths.rotate(pos, new UnityEngine.Vector4(1, 0, 0, 0),
//                IMRE.ScaleDimension.SpencerStudyControl.ins.NorthPole,
//                IMRE.Math.Operations.Angle(new UnityEngine.Vector4(1, 0, 0, 0),
//                    IMRE.ScaleDimension.SpencerStudyControl.ins.NorthPole));

            //take stereographic projection in a given direction.
            //this assumes that the north pole is in the (x) direction.
            //from wikipedia
            float denom = 1 - pos.x;
            Unity.Mathematics.float3 result = UnityEngine.Mathf.Pow(pos.y / denom, 2) +
                                              UnityEngine.Mathf.Pow(pos.z / denom, 2) +
                                              UnityEngine.Mathf.Pow(pos.w / denom, 2);

            //TODO turn to restore north
            //HigherDimensionsMaths.rotate(result, new Vector4(1, 0, 0, 0),
            //    IMRE.ScaleDimension.SpencerStudyControl.ins.NorthPole,
            //    -1f*Operations.Angle(new Vector4(1, 0, 0, 0), IMRE.ScaleDimension.SpencerStudyControl.ins.NorthPole));
            return result;
        }

        public static Unity.Mathematics.float3 parallelProjection(Unity.Mathematics.float4 tmpVert)
        {
            Unity.Mathematics.float4 planePos = IMRE.ScaleDimension.SpencerStudyControl.ins.hyperPlane;
            Unity.Mathematics.float4 planeNormal = IMRE.ScaleDimension.SpencerStudyControl.ins.hyperPlaneNormal;

            Unity.Mathematics.float4 pos =
                tmpVert - IMRE.Math.HigherDimensionsMaths.project(tmpVert - planePos, planeNormal);

            //this basis system might be unstable as the axis is moved.  Consider defining axis by basis.
            Unity.Mathematics.float4x3 basis = IMRE.Math.HigherDimensionsMaths.basisSystem(planeNormal);
            return new Unity.Mathematics.float3(UnityEngine.Vector4.Dot(pos, basis.c0),
                UnityEngine.Vector4.Dot(pos, basis.c1), UnityEngine.Vector4.Dot(pos, basis.c2));
        }

        public static Unity.Mathematics.float3 projectiveProjection(Unity.Mathematics.float4 tmpVert)
        {
            Unity.Mathematics.float4 origin = IMRE.ScaleDimension.SpencerStudyControl.ins.origin;
            Unity.Mathematics.float4 planePos = IMRE.ScaleDimension.SpencerStudyControl.ins.hyperPlane;
            Unity.Mathematics.float4 planeNormal = IMRE.ScaleDimension.SpencerStudyControl.ins.hyperPlaneNormal;

            //normalized direction
            Unity.Mathematics.float4 dir = (tmpVert - origin) / IMRE.Math.Operations.magnitude(tmpVert - origin);
            Unity.Mathematics.float4 projOnPlane =
                tmpVert - IMRE.Math.HigherDimensionsMaths.project(tmpVert - planePos, planeNormal);
            //TODO consider if mathf.cos takes an absolute value here
            float diff = IMRE.Math.Operations.magnitude(tmpVert - projOnPlane) /
                         UnityEngine.Mathf.Cos(IMRE.Math.Operations.Angle(planeNormal, dir));
            Unity.Mathematics.float4 pos = tmpVert + diff * dir;

            //this basis system might be unstable as the axis is moved.  Consider defining axis by basis.
            Unity.Mathematics.float4x3 basis = IMRE.Math.HigherDimensionsMaths.basisSystem(planeNormal);
            return new Unity.Mathematics.float3(UnityEngine.Vector4.Dot(pos, basis.c0),
                UnityEngine.Vector4.Dot(pos, basis.c1), UnityEngine.Vector4.Dot(pos, basis.c2));
        }

        public static Unity.Mathematics.float3 projectPosition(Unity.Mathematics.float4 pos)
        {
            switch (IMRE.ScaleDimension.SpencerStudyControl.ins.projectionMethod)
            {
                case IMRE.Math.ProjectionMethod.stereographic:
                    return stereographicProjection(pos);
                case IMRE.Math.ProjectionMethod.projective:
                    return projectiveProjection(pos);
                case IMRE.Math.ProjectionMethod.parallel:
                    return parallelProjection(pos);
                default:
                    UnityEngine.Debug.LogWarning("Failed to Project, Method not Implemented");
                    return Unity.Mathematics.float3.zero;
            }
        }
    }
}