﻿using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections
{
    public class TriangleRenderer : MonoBehaviour
    {
        public Material mat;
        // Start is called before the first frame update
        void Start()
        {
            Unity.Mathematics.float3x3 vertices =
                GetComponentInParent<IMRE.ScaleDimension.CrossSections.TriangleCrossSection>().triangleVerticies;
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();

            lr.positionCount = 3;
            lr.SetPositions(new Vector3[] {vertices.c0, vertices.c1, vertices.c2});
    
            lr.useWorldSpace = true;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.loop = true;

            lr.material = mat;
        }
    }
}
