using System;
using IMRE.Math;
using Unity.Mathematics;
using UnityEngine;

namespace IMRE.HandWaver.ScaleDimension
{
    public static class MeshOperations
    {
	    public static void projectTriangle(float4[] verts, int subdiv, out float3[] out_verts, out int[] out_tris,
		    ProjectionMethod method)
	    {
		    //break triangle into smaller triangles
		    //use a modified Sierpiński triangle as a division mechanic (partition every triangle into 3 triangles each round). Then there are always 3^n triangles
		    float4[] tmp_verts = verts;
		    out_verts = new float3[tmp_verts.Length];
		    out_tris = new int[] {0, 1, 2};
		    for (int i = 0; i < subdiv; i++)
		    {

			    subdivideVerts(tmp_verts, out_tris, out tmp_verts, out_tris);
		    }

		    for (int i = 0; i < tmp_verts.Length; i++)
		    {
			    switch (method)
			    {
				    case ProjectionMethod.stereographic:
					    out_verts[i] = stereographicProjection(tmp_verts[i]);
					    break;
				    case ProjectionMethod.orthographic:
					    break;
				    case ProjectionMethod.projective:
					    break;
				    case ProjectionMethod.parallel:
					    break;
				    default:
					    throw new ArgumentOutOfRangeException(nameof(method), method, null);
			    }
		    }
	    }

	    private static float4[] subdivideVerts(float4[] verts, int[] tris, out float4[] out_verts, out int[] out_tris)
        {
	        //recursive case.
            out_verts = new float4[verts.Length * 2];
			out_tris = new int[tris.Length*4];
            if (verts.Length > 3)
            {
				float4[] tmpVerts = new float4[verts.Length];
				int[] tempTris;
				subdivideVerts(verts.GetRange(0, (verts.Length / 2) - 1), tmpVerts, tempTris);
				tmpVerts.CopyToRange(out_verts, 0,verts.Length-1);
               
				for ( i = 0; i < tmpTris.Length, i ++)
				{
					outTris[i] = tmpTris[i];
				}

				result[verts.Length] = subdivideVerts(verts.GetRange((verts.Length / 2), verts.Length - 1), tmpVerts,
					tempTris);
				tmpVerts.CopyToRange(out_verts, verts.Length, verts.Length*2-1);

				for ( i = 0; i < tmpTris.Length, i ++)
				{
					outTris[i + tmpTris.Length] = tmpTris[i] + verts.Length;
				}
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
				
				out_tris = {
					0,3,5,
					1,3,4,
					3,4,5,
					4,5,2
				};
            }
        }

	    public static float3 stereographicProjection(float4 pos)
	    {
		    //then inflate each vert to the edge of the sphere and keep that value.
		    pos = pos / Operations.magnitude(pos);

		    //take stereographic projection in a given direction.
		    //this assumes that the north pole is in the (x) direction.
		    float denom = 1- pos.x;
		    return Mathf.Pow(pos.y / denom, 2) + Mathf.Pow(pos.z / denom, 2) +
		                   Mathf.Pow(pos.w / denom, 2);
	    }
    }
}