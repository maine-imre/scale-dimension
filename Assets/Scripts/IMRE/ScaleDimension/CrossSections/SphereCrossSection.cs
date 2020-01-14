using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMRE.ScaleDimension.CrossSections
{
    public class SphereCrossSection : AbstractCrossSection
    {
        public float radius;
        public Vector3 center;
        public int n = 5;
        private UnityEngine.LineRenderer circleRenderer => GetComponent<UnityEngine.LineRenderer>();

        private void Start()
        {
            #region Render

            gameObject.AddComponent<UnityEngine.LineRenderer>();
            circleRenderer.material = mat;
            circleRenderer.startWidth = .005f;
            circleRenderer.endWidth = .005f;
            
            #endregion
        }

        private void Update()
        {
            float r = Mathf.Sqrt(Mathf.Pow(radius,2)-Mathf.Pow(Vector3.Project(center-planePos,planeNormal).magnitude,2));
            
            Vector3 norm1 = Vector3.Cross(planeNormal, Vector3.up);
            if (norm1.magnitude == 0)
            {
                norm1 = Vector3.Cross(planeNormal, Vector3.right);
            }
            Vector3 norm2 = Vector3.Cross(planeNormal, norm1);
            norm1.Normalize();
            norm2.Normalize();
            
            renderCircle(norm1,norm2, r);
        }

        /// <summary>
        ///     Function to render the outline of a circle
        /// </summary>
        public void renderCircle(UnityEngine.Vector3 norm1, UnityEngine.Vector3 norm2, float radius)
        {

            //array of vector3s for vertices
            UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[n];

            //math for rendering circle
            for (int i = 0; i < n; i++)
            {
                vertices[i] = radius * (UnityEngine.Mathf.Sin(i * UnityEngine.Mathf.PI * 2 / (n - 1)) * norm1) +
                              radius * (UnityEngine.Mathf.Cos(i * UnityEngine.Mathf.PI * 2 / (n - 1)) * norm2) + center;
            }

            //Render circle
            UnityEngine.LineRenderer lineRenderer = GetComponent<UnityEngine.LineRenderer>();
            //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.startColor = UnityEngine.Color.blue;
            lineRenderer.endColor = UnityEngine.Color.blue;
            lineRenderer.startWidth = IMRE.ScaleDimension.SpencerStudyControl.lineRendererWidth;
            lineRenderer.endWidth = IMRE.ScaleDimension.SpencerStudyControl.lineRendererWidth;
            lineRenderer.positionCount = n;
            lineRenderer.SetPositions(vertices);
        }
    }
}