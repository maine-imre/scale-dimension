namespace IMRE.ScaleDimension
{
    /// <summary>
    ///     Central control for scale and dimension study.
    ///     also includes logic to make a slider out of MasterGeoObjs
    /// </summary>
    public class SpencerStudyControl : UnityEngine.MonoBehaviour
    {
        public static SpencerStudyControl ins;

        /// <summary>
        ///     The number of degrees that each vertex is folded by.
        ///     Consider changing to percent;
        /// </summary>
        internal static float percentFolded;

        public static bool debugRendererXC;
        public static float lineRendererWidth = 0.001f;

        private System.Collections.Generic.List<UnityEngine.GameObject> _sliderInputs;
        public System.Collections.Generic.List<UnityEngine.GameObject> allFigures;

        /// <summary>
        ///     An override that automatically animates the slider and the folding process
        /// </summary>
        public bool animateFold;

        public bool animateUp = true;

        /// <summary>
        ///     A boolean for debugging that allows the fold to be manipulated in the editor at play
        /// </summary>
        public bool foldOverride;

        /// <summary>
        ///     The override value with a slider in the editor.
        /// </summary>
        [UnityEngine.RangeAttribute(0, 1)] public float foldOverrideValue;

        /// <summary>
        ///     A point on the 3-plane we are projecting onto
        /// </summary>
        public Unity.Mathematics.float4 hyperPlane;

        //Projection Information
        //Stereographic data
        /// <summary>
        ///     The north pole for the 3-sphere.  Assume radius = 1
        /// </summary>
        public Unity.Mathematics.float4 NorthPole;

        /// <summary>
        ///     The normal direction for the plane for oblique
        /// </summary>
        public Unity.Mathematics.float4 obliquePlaneNormal;

        //Projective and Parallel Data.  Note origin is not used for parallel projection
        /// <summary>
        ///     The origin of the projection.
        ///     Should this be the user's "eye"
        /// </summary>
        public Unity.Mathematics.float4 origin;

        public bool orthographicOverride;

        public UnityEngine.GameObject pointPrefab;
        public IMRE.Math.ProjectionMethod projectionMethod;

        public int subdiv = 1;

        /// <summary>
        ///     Auto-overrides for orthographic projection
        /// </summary>
        public Unity.Mathematics.float4 hyperPlaneNormal =>
            orthographicOverride
                ? (hyperPlane - origin) / IMRE.Math.Operations.magnitude(hyperPlane - origin)
                : obliquePlaneNormal;

        private void Start()
        {
            ins = this;

            //_sliderInputs = allFigures.OfType<ISliderInput>().ToList();
            _sliderInputs =
                System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(allFigures,
                    go => go.GetComponent(typeof(ISliderInput)) != null));
        }

        private void Update()
        {
            float percent;
            //if the override bool is set, use in editor override value

            //if the boolean is set to animate the figure
            if (animateFold)
            {
                //increment the degree folded by one degree. 
                percentFolded = animateUp ? percentFolded + .01f : percentFolded - .01f;

                if (percentFolded >= 1)
                {
                    percentFolded = 1f; //round to whole
                    animateFold = false;
                    animateUp = false;
                }
                else if (percentFolded <= 0)
                {
                    percentFolded = 0f;
                    animateFold = false;
                    animateUp = true;
                }

                percent = percentFolded;

                //IMRE.EmbodiedUserInput.TouchSlider.ins.SliderValue = percent;
            }
            else if (foldOverride)
            {
                percent = foldOverrideValue;
                //currenlty no touch slider.
                //IMRE.EmbodiedUserInput.TouchSlider.ins.SliderValue = percent;
            }
            // if the participant is directly manipulating the slider
            else
            {
                //TODO fix this.
                percent = 0f;
                //currently no touch slider.
                //percent = IMRE.EmbodiedUserInput.TouchSlider.ins.SliderValue;
            }

            setPercentFolded(percent);
        }

        private void setPercentFolded(float percent)
        {
            foldOverrideValue = percent;

            _sliderInputs.ForEach(
                si =>
                {
                    if (si.activeSelf)
                        si.GetComponent<ISliderInput>().slider = percent;
                }
            );
        }
    }
}