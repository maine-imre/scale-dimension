using System.Linq;
using IMRE.HandWaver.ScaleDimension;
using Unity.Mathematics;
using UnityEngine;

namespace IMRE.ScaleDimension.Nets
{
    public abstract class net3D : UnityEngine.MonoBehaviour, ISliderInput
    {
        private float _percentFolded;

        public bool sliderOverride;

        //public DateTime startTime;
        public UnityEngine.Mesh mesh => GetComponent<UnityEngine.MeshFilter>().mesh;

        public UnityEngine.LineRenderer lineRenderer => GetComponent<UnityEngine.LineRenderer>();

        public float PercentFolded
        {
            get => _percentFolded;
            //set positions for linerenderer and vertices for mesh
            set
            {
                //set vertices on line segment
                _percentFolded = value;
                lineRenderer.SetPositions(projectedVerts(lineRendererVerts(_percentFolded)));
                mesh.SetVertices(projectedVerts(meshVerts(_percentFolded)).ToList());
                //TODO consider if mesh needs to be subdivided or if line renderer needs to be subidivded.
            }
        }

        public float slider
        {
            set => PercentFolded = !sliderOverride ? value : 1f;
        }

        /// <summary>
        ///     abstract function for positioning mesh vertices for rendering based on percent that the net is folded
        /// </summary>
        /// <param name="percentFolded"></param>
        /// <returns></returns>
        public abstract UnityEngine.Vector3[] meshVerts(float percentFolded);

        private Vector3[] projectedVerts(Vector3[] verts)
        {
            float4[] out_verts = new float4[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                out_verts[i] = new float4(verts[i].x, verts[i].y, verts[i].z, 0f);
            }

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = MeshOperations.projectPosition(out_verts[i]);
            }

            return verts;
        }

        /// <summary>
        ///     abstract function for positioning line renderers based on percent that the net is folded
        /// </summary>
        /// <param name="percentFolded"></param>
        /// <returns></returns>
        public abstract UnityEngine.Vector3[] lineRendererVerts(float percentFolded);
    }
}