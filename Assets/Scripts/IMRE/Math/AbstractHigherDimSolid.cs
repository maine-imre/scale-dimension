namespace IMRE.Math
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.MeshFilter))]
    [UnityEngine.RequireComponent(typeof(UnityEngine.MeshRenderer))]
    /// <summary>
    /// Higher dimensional solids and their projections automated.
    /// Now uses sliers for scale and dimension study, via a dictionary.
    /// </summary>
    public abstract class AbstractHigherDimSolid : UnityEngine.MonoBehaviour
    {
        internal Unity.Mathematics.float4[] originalVertices;

        public abstract UnityEngine.Vector2[] uvs { get; }
        public abstract int[] triangles { get; }
        public abstract UnityEngine.Color[] colors { get; }

        private void Update()
        {
            GetComponent<UnityEngine.MeshFilter>().mesh =
                IMRE.HandWaver.ScaleDimension.MeshOperations.projectedMesh(originalVertices, triangles);
            //GetComponent<UnityEngine.MeshFilter>().mesh.colors = colors;
        }
    }
}