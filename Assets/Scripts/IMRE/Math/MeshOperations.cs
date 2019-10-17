using UnityEngine.Mathematics;

namespace IMRE.HandWaver.ScaleDimension
{
    public static class MeshOperations
    {
        public static void projectTriangle(float4[] verts, int subdiv, out float4[] out_verts, out int out_tris, ProjectionMethod method)
        {
            //break triangle into smaller triangles
            //use a modified Sierpiński triangle as a division mechanic (partition every triangle into 3 triangles each round). Then there are always 3^n triangles
			out_verts = verts;
			out_tris = {0,1,2};
            for (int i = 0; i < subdiv; i++)
            {
                
				subdivideVerts(out_verts, out_tris, out_verts, out_tris);
            }
			for(int i = 0; i < out_verts.Length; i++){
				switch(method){
					case ProjectionMethod.stereographic:
					//then inflate each vert to the edge of the sphere and keep that value.
					out_verts[i] = out_verts[i].normalize;
					
					//take stereographic projection in a given direction.
					
					break;
				}
			}
        }

        private static float4[] subdivideVerts(float4[] verts, int[] tris, out float4[] out_verts, out int[] out_tris)
        {
            out_verts = new float4[verts.Length * 2];
			out_tris = new int[tris.Length*4];
            if (verts.Length > 3)
            {
				float4[] tmpVerts = new float4[verts.Length];
				int[] tempTris = new int[tris.Length*4];
                subdivideTriangle(verts.GetRange(0, (verts.Length / 2)-1)), tmpVerts, tmpTris);
				tmpVerts.CopyToRange(out_verts, 0,verts.Length-1);
               
				for ( i = 0; i < tmpTris.Length, i ++){
					outTris[i] = tmpTris[i];
				}

 				result[verts.Length] = subdivideTriangle(verts.GetRange((verts.Length / 2), verts.Length -1), tmpVerts, tmpTris);
				tmpVerts.CopyToRange(out_verts, verts.Length, verts.Length*2-1);

				for ( i = 0; i < tmpTris.Length, i ++){
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
    }
}