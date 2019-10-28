using IMRE.HandWaver.ScaleDimension;
using Unity.Mathematics;
using UnityEngine;

namespace IMRE.ScaleDimension.Nets
{
    public abstract class net2D : UnityEngine.MonoBehaviour, ISliderInput
    {
        private float _percentFolded;

        public System.Collections.Generic.List<UnityEngine.GameObject> foldPoints =
            new System.Collections.Generic.List<UnityEngine.GameObject>();

        public bool sliderOverride;

        /// <summary>
        ///     float to set fold percentage to position vertices
        /// </summary>
        public float PercentFolded
        {
            get => _percentFolded;

            set
            {
                //set vertices using vert function
                _percentFolded = value;
                GetComponent<UnityEngine.LineRenderer>().SetPositions(projectedVerts(_percentFolded));
                //TODO consider if the line renderer needs to be subdivided.
            }
        }

        /// <summary>
        ///     slider to control fold percent
        /// </summary>
        public float slider
        {
            set => PercentFolded = !sliderOverride ? value : 1f;
        }

        private Vector3[] projectedVerts(float percentFolded)
        {
            Vector3[] verts = this.verts(percentFolded);

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
        ///     abstract function for positioning vertices based on how folded the 2d net is
        /// </summary>
        /// <param name="percentFolded"></param>
        /// <returns></returns>
        public abstract UnityEngine.Vector3[] verts(float percentFolded);
    }
}