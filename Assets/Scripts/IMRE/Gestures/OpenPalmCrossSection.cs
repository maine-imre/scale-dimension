namespace IMRE.Gestures
{
    public class OpenPalmCrossSection : UnityEngine.MonoBehaviour
    {
        private System.Collections.Generic.List<IMRE.ScaleDimension.CrossSections.AbstractCrossSection> _crosssections;
        public bool manualControl;
        public Unity.Mathematics.float3 normal;
        public Unity.Mathematics.float3 pos;

        public Valve.VR.SteamVR_Behaviour_Pose pose;

        private void Start()
        {
            _crosssections =
                System.Linq.Enumerable.ToList(
                    FindObjectsOfType<IMRE.ScaleDimension.CrossSections.AbstractCrossSection>());
            pose.onTransformChanged.AddListener(updateCrossSections);
        }

        private void Update()
        {
            if (manualControl) setCrossSections(pos, normal);
        }

        private void updateCrossSections(Valve.VR.SteamVR_Behaviour_Pose arg0, Valve.VR.SteamVR_Input_Sources arg1)
        {
            pos = pose.origin.position;
            normal = pose.origin.up;
            setCrossSections(pose.origin.position, pose.origin.up);
        }

        private void setCrossSections(Unity.Mathematics.float3 pos, Unity.Mathematics.float3 normal)
        {
            _crosssections.ForEach(xc => xc.planePos = pos);
            _crosssections.ForEach(xc => xc.planeNormal = normal);
        }
    }
}