public class axesControl : UnityEngine.MonoBehaviour
{
    private static readonly Unity.Mathematics.float4 right = new Unity.Mathematics.float4(1f, 0f, 0f, 0f);
    private static readonly Unity.Mathematics.float4 up = new Unity.Mathematics.float4(0f, 1f, 0f, 0f);
    private static readonly Unity.Mathematics.float4 forward = new Unity.Mathematics.float4(0f, 0f, 1f, 0f);
    private static readonly Unity.Mathematics.float4 wforward = new Unity.Mathematics.float4(0f, 0f, 0f, 1f);
    public System.Collections.Generic.List<UnityEngine.LineRenderer> axes;
    public System.Collections.Generic.List<UnityEngine.Color> axesColors;

    public bool scaleCondition;

    private static Unity.Mathematics.float4[] endpoints => new[] {right, up, forward, wforward};

    private float scale => scaleCondition ? .1f : 10f;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            axes[i].startWidth = 0.001f;
            axes[i].endWidth = 0.001f;
            axes[i].useWorldSpace = true;
            axes[i].startColor = axesColors[i];
            axes[i].endColor = axesColors[i];
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Unity.Mathematics.float3 zero = IMRE.HandWaver.ScaleDimension.MeshOperations.projectPosition(Unity.Mathematics.float4.zero);
        for (int i = 0; i < 4; i++)
        {
            axes[i].SetPosition(0, scale * zero);
            axes[i].SetPosition(1, scale * IMRE.HandWaver.ScaleDimension.MeshOperations.projectPosition(endpoints[i]));
        }
    }
}