using System.Linq;
using UnityEngine;

namespace IMRE.Gestures
{
    public class OpenPalmCrossSection : MonoBehaviour
    {
        private System.Collections.Generic.List<IMRE.ScaleDimension.CrossSections.AbstractCrossSection> _crosssections;

        public Valve.VR.SteamVR_Behaviour_Pose pose;
    
        private void Start()
        {
            _crosssections = GameObject.FindObjectsOfType<IMRE.ScaleDimension.CrossSections.AbstractCrossSection>().ToList();
            pose.onTransformChanged.AddListener(updateCrossSections);
        }

        private void updateCrossSections(Valve.VR.SteamVR_Behaviour_Pose arg0, Valve.VR.SteamVR_Input_Sources arg1)
        {
            setCrossSections(pose.origin.position, pose.origin.up);
        }

        private void setCrossSections(Unity.Mathematics.float3 pos, Unity.Mathematics.float3 normal)
        {
            _crosssections.ForEach(xc => xc.planePos = pos);
            _crosssections.ForEach(xc => xc.planeNormal = normal);
        }
    }
}
