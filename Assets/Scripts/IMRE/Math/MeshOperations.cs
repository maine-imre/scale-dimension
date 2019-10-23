namespace IMRE.HandWaver.ScaleDimension
{
    public static class MeshOperations
    {
        public static UnityEngine.Mesh projectedMesh(Unity.Mathematics.float4[] verts, int[] tris)
        {
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            UnityEngine.Vector3[] verticies = new UnityEngine.Vector3[1];
            int[] triangles = new int[1];
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
                if (i == 0)
                {
                    verticies = new UnityEngine.Vector3[tmpVerts.Length * (tris.Length / 3)];
                    triangles = new int[tmpTris.Length * (tris.Length / 3)];
                    //normals = new Vector3[(tmpTris.Length/3)*(tris.Length/3)];
                }

                tmpVerts.CopyTo(verticies, i * tmpVerts.Length);
                tmpTris.CopyTo(verticies, i * tmpTris.Length);
                //TODO handle uvs;
                //TODO handle normals;
            }

            mesh.vertices = verticies;
            mesh.triangles = triangles;
            //mesh.uv = uvs;
            //mesh.normals = normals;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static void projectTriangle(Unity.Mathematics.float4[] verts, int subdiv,
            out Unity.Mathematics.float3[] out_verts, out int[] out_tris,
            IMRE.Math.ProjectionMethod method)
        {
            //break triangle into smaller triangles
            //use a modified Sierpiński triangle as a division mechanic (partition every triangle into 3 triangles each round). Then there are always 3^n triangles
            Unity.Mathematics.float4[] tmp_verts = verts;
            out_verts = new Unity.Mathematics.float3[tmp_verts.Length];
            out_tris = new[] {0, 1, 2};
            for (int i = 0; i < subdiv; i++) subdivideVerts(tmp_verts, out_tris, out tmp_verts, out out_tris);

            for (int i = 0; i < tmp_verts.Length; i++)
            {
                switch (method)
                {
                    case IMRE.Math.ProjectionMethod.stereographic:
                        out_verts[i] = stereographicProjection(tmp_verts[i]);
                        break;
                    case IMRE.Math.ProjectionMethod.projective:
                        out_verts[i] = projectiveProjection(tmp_verts[i]);
                        break;
                    case IMRE.Math.ProjectionMethod.parallel:
                        out_verts[i] = parallelProjection(tmp_verts[i]);
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(method), method, null);
                }
            }
        }

        private static void subdivideVerts(Unity.Mathematics.float4[] verts, int[] tris,
            out Unity.Mathematics.float4[] out_verts, out int[] out_tris)
        {
            //recursive case.
            out_verts = new Unity.Mathematics.float4[verts.Length * 2];
            out_tris = new int[tris.Length * 4];
            if (verts.Length > 3)
            {
                Unity.Mathematics.float4[] tmpVerts;
                int[] tempTris;
                subdivideVerts(verts.GetRange(0, verts.Length / 2 - 1), tris, out tmpVerts, out tempTris);
                tmpVerts.CopyToRange(ref out_verts, 0, verts.Length - 1);
                tempTris.CopyTo(out_tris, 0);

                //symmetric case
                subdivideVerts(verts.GetRange(verts.Length / 2, verts.Length - 1),
                    tris, out tmpVerts, out tempTris);
                tmpVerts.CopyToRange(ref out_verts, verts.Length, verts.Length * 2 - 1);
                tempTris.CopyTo(out_tris, tempTris.Length);
            }
            else
            {
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

            //turn to make north pole float4.right
            IMRE.Math.HigherDimensionsMaths.rotate(pos, new UnityEngine.Vector4(1, 0, 0, 0),
                IMRE.ScaleDimension.SpencerStudyControl.ins.NorthPole,
                IMRE.Math.Operations.Angle(new UnityEngine.Vector4(1, 0, 0, 0),
                    IMRE.ScaleDimension.SpencerStudyControl.ins.NorthPole));

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

        private static Unity.Mathematics.float3 parallelProjection(Unity.Mathematics.float4 tmpVert)
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

        private static Unity.Mathematics.float3 projectiveProjection(Unity.Mathematics.float4 tmpVert)
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
    }
}