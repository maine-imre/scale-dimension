using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiveCellCrossSection : IMRE.ScaleDimension.CrossSections.AbstractCrossSection
{
    public Unity.Mathematics.float4[] fiveCellVertices;
    public List<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection> pyramidXC = 
        new List<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>();

    
    // Start is called before the first frame update
    void Start()
    {
        Unity.Mathematics.float4 a = fiveCellVertices[0];
        Unity.Mathematics.float4 b = fiveCellVertices[1];
        Unity.Mathematics.float4 c = fiveCellVertices[2];
        Unity.Mathematics.float4 d = fiveCellVertices[3];
        Unity.Mathematics.float4 e = fiveCellVertices[4];

        Unity.Mathematics.float4[] pyrmaid1 = {a, b, c, d};
        Unity.Mathematics.float4[] pyrmaid2 = {a, b, d, e};
        Unity.Mathematics.float4[] pyrmaid3 = {a, c, d, e};
        Unity.Mathematics.float4[] pyrmaid4 = {b, c, d, e};
        
        BuildTetrahedron(pyrmaid1,planePos,planeNormal);
        BuildTetrahedron(pyrmaid2,planePos,planeNormal);
        BuildTetrahedron(pyrmaid3,planePos,planeNormal);
        BuildTetrahedron(pyrmaid4,planePos,planeNormal);
        
    }
    
    private void BuildTetrahedron(Unity.Mathematics.float4[] vertices, Unity.Mathematics.float3 planePos,
        Unity.Mathematics.float3 planeNorm)
    {
        UnityEngine.GameObject tet1_go = new UnityEngine.GameObject();
        tet1_go.transform.parent = this.transform;
        tet1_go.AddComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>();
        pyramidXC.Add(tet1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>());
        
        //TODO project verticies down a dimension.
        //tet1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().tetrahderonVertices = vertices;
        tet1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().planePos = planePos;
        tet1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().planeNormal = planeNorm;
        tet1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().mat = mat;
    }
    
    private void UpdateTriangles()
    {
        Unity.Mathematics.float4 a = fiveCellVertices[0];
        Unity.Mathematics.float4 b = fiveCellVertices[1];
        Unity.Mathematics.float4 c = fiveCellVertices[2];
        Unity.Mathematics.float4 d = fiveCellVertices[3];
        Unity.Mathematics.float4 e = fiveCellVertices[4];

        Unity.Mathematics.float4[] pyrmaid1 = {a, b, c, d};
        Unity.Mathematics.float4[] pyrmaid2 = {a, b, d, e};
        Unity.Mathematics.float4[] pyrmaid3 = {a, c, d, e};
        Unity.Mathematics.float4[] pyrmaid4 = {b, c, d, e};
            
        
        pyramidXC.ForEach(p => p.planePos = planePos);
        pyramidXC.ForEach(p => p.planeNormal = planeNormal);
        //TODO project verticies down a dimension.
        //pyramidXC[0].tetrahderonVertices = pyrmaid1;
        //pyramidXC[1].tetrahderonVertices = pyrmaid2;
        //pyramidXC[2].tetrahderonVertices = pyrmaid3;
        //pyramidXC[3].tetrahderonVertices = pyrmaid4;   
    }
}
