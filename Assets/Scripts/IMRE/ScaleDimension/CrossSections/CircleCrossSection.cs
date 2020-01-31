namespace IMRE.ScaleDimension.CrossSections
{
    /// <summary>
    ///     cross-section of a circle represented by two points where intersection occurs in circle
    ///     or one point if it only hits an edge
    /// </summary>
    public class CircleCrossSection : AbstractCrossSection
    {
        
        public float radius = 1f;
        public Unity.Mathematics.float3 circlePlane = new Unity.Mathematics.float3(0,1,0);
        public UnityEngine.Vector3 circleCenter = new Unity.Mathematics.float3(0,1,0);

        // Start is called before the first frame update
        private void Start()
        {
            #region Render

            gameObject.AddComponent<UnityEngine.LineRenderer>();
            circleRenderer.material = mat;
            circleRenderer.startWidth = .005f;
            circleRenderer.endWidth = .005f;
            circleRenderer.enabled = debugRenderer;

            UnityEngine.GameObject child = new UnityEngine.GameObject();
            child.transform.parent = transform;
            child.AddComponent<UnityEngine.LineRenderer>();
            crossSectionRenderer.material = crossSectionMaterial;
            crossSectionRenderer.startWidth = SpencerStudyControl.lineRendererWidth;
            crossSectionRenderer.endWidth = SpencerStudyControl.lineRendererWidth;

            crossSectionRenderer.enabled = debugRenderer;

            //generate four points to show crossSections.
            crossSectionPoints.Add(Instantiate(SpencerStudyControl.ins.pointPrefab));
            crossSectionPoints.Add(Instantiate(SpencerStudyControl.ins.pointPrefab));
            crossSectionPoints.ForEach(p => p.transform.SetParent(transform));

            #endregion
        }

        private void Update()
        {
            //fine line that intersects circle
            
            
        }

        UnityEngine.Experimental.PlayerLoop.Update

        /// <summary>
        ///     Function to render the outline of a circle
        /// </summary>
        public void renderCircle(UnityEngine.Vector3 norm1, UnityEngine.Vector3 norm2)
        {

            //array of vector3s for vertices
            UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[n];

            //math for rendering circle
            for (int i = 0; i < n; i++)
            {
                vertices[i] = radius * (UnityEngine.Mathf.Sin(i * UnityEngine.Mathf.PI * 2 / (n - 1)) * norm1) +
                              radius * (UnityEngine.Mathf.Cos(i * UnityEngine.Mathf.PI * 2 / (n - 1)) * norm2 + circleCenter);
            }

            //Render circle
            UnityEngine.LineRenderer lineRenderer = GetComponent<UnityEngine.LineRenderer>();
            //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.startColor = UnityEngine.Color.blue;
            lineRenderer.endColor = UnityEngine.Color.blue;
            lineRenderer.startWidth = SpencerStudyControl.lineRendererWidth;
            lineRenderer.endWidth = SpencerStudyControl.lineRendererWidth;
            lineRenderer.positionCount = n;
            lineRenderer.SetPositions(vertices);
        }

        #region Variables/Components

        public int n = 5;

        private UnityEngine.LineRenderer circleRenderer => GetComponent<UnityEngine.LineRenderer>();

        private UnityEngine.LineRenderer crossSectionRenderer =>
            transform.GetChild(0).GetComponent<UnityEngine.LineRenderer>();

        public UnityEngine.Material circleMaterial;
        public UnityEngine.Material crossSectionMaterial;
        public bool debugRenderer = SpencerStudyControl.debugRendererXC;

        public System.Collections.Generic.List<UnityEngine.GameObject> crossSectionPoints =
            new System.Collections.Generic.List<UnityEngine.GameObject>();

        #endregion
    }
}