namespace IMRE.ScaleDimension.CrossSections
{
    public class AbstractCrossSection : UnityEngine.MonoBehaviour
    {
        public UnityEngine.Material mat;
        internal Unity.Mathematics.float3 planeNormal;

        internal Unity.Mathematics.float3 planePos;
    }
}