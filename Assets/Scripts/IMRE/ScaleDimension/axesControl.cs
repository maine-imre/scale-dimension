using System.Collections;
using System.Collections.Generic;
using IMRE.Math;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

public class axesControl : MonoBehaviour
{
    public List<LineRenderer> axes;
    private static float4 right = new float4(1f,0f,0f,0f);
    private static float4 up = new float4(0f, 1f, 0f, 0f);
    private static float4 forward = new float4(0f,0f,1f,0f);
    private static float4 wforward = new float4(0f,0f,0f,1f);
    public List<Color> axesColors;

    private static float4[] endpoints
    {
        get
        {
            return new float4[] {right, up, forward, wforward};
        }
    }
    
    

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            axes[i].startWidth = 0.001f;
            axes[i].endWidth = 0.001f;
            axes[i].useWorldSpace = true;
            axes[i].startColor = axesColors[i];
            axes[i].endColor = axesColors[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        float3 zero = projectPosition(float4.zero);
        for(int i = 0; i < 4; i++)
        {
            axes[i].SetPosition(0,zero);
            axes[i].SetPosition(1,projectPosition(endpoints[i]));
        }
    }

    private float3 projectPosition(float4 pos)
    {
        return IMRE.Math.HigherDimensionsMaths.projectDownDimension(pos,
            inputBasis, method, Vangle, eyePosition, viewingRadius);
    }

    public float viewingRadius;

    public float4 eyePosition;

    public float Vangle;

    public ProjectionMethod method;

    public float4x3 inputBasis;
}
