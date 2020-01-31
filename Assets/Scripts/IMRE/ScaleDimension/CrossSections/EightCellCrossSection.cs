using Unity.Mathematics;
using System.Collections.Generic;

namespace IMRE.ScaleDimension.CrossSections
{
    public class EightCellCrossSection : AbstractCrossSection
    {
        public Unity.Mathematics.float4[] eightCellVertices;
        public List<IMRE.ScaleDimension.CrossSections.CubeCrossSection> hyperCubeXC = new List<CubeCrossSection>();

        private void Start()
        {
            float4 a = eightCellVertices[0];
            float4 b = eightCellVertices[1];
            float4 c = eightCellVertices[2];
            float4 d = eightCellVertices[3];
            float4 e = eightCellVertices[4];
            float4 f = eightCellVertices[5];
            float4 g = eightCellVertices[6];
            float4 h = eightCellVertices[7];
            float4 i = eightCellVertices[8];
            float4 j = eightCellVertices[9];
            float4 k = eightCellVertices[10];
            float4 l = eightCellVertices[11];
            float4 m = eightCellVertices[12];
            float4 n = eightCellVertices[13];
            float4 o = eightCellVertices[14];
            float4 p = eightCellVertices[15];

            //TODO: organize these correctly
            float4[] cube1 = {a, b, c, d, e, f, g, h};
            float4[] cube2 = {a, b, c, d, e, f, g, h};
            float4[] cube3 = {a, b, c, d, e, f, g, h};
            float4[] cube4 = {a, b, c, d, e, f, g, h};
            float4[] cube5 = {a, b, c, d, e, f, g, h};
            float4[] cube6 = {a, b, c, d, e, f, g, h};
            float4[] cube7 = {a, b, c, d, e, f, g, h};
            float4[] cube8 = {a, b, c, d, e, f, g, h};

            BuildCube(cube1, planePos, planeNormal);
            BuildCube(cube2, planePos, planeNormal);
            BuildCube(cube3, planePos, planeNormal);
            BuildCube(cube4, planePos, planeNormal);
            BuildCube(cube5, planePos, planeNormal);
            BuildCube(cube6, planePos, planeNormal);
            BuildCube(cube7, planePos, planeNormal);
            BuildCube(cube8, planePos, planeNormal);
        }

        private void BuildCube(Unity.Mathematics.float4[] vertices, Unity.Mathematics.float3 planePos,
            Unity.Mathematics.float3 planeNorm)
        {
            UnityEngine.GameObject cube1_go = new UnityEngine.GameObject();
            cube1_go.transform.parent = this.transform;
            cube1_go.AddComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>();
            hyperCubeXC.Add(cube1_go.GetComponent<IMRE.ScaleDimension.CrossSections.CubeCrossSection>());

            //TODO project verticies down a dimension.
            //cube1_go.GetComponent<IMRE.ScaleDimension.CrossSections.CubeCrossSecion>().cubeVertices = vertices;
            cube1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().planePos = planePos;
            cube1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().planeNormal = planeNorm;
            cube1_go.GetComponent<IMRE.ScaleDimension.CrossSections.TetrahedronCrossSection>().mat = mat;
        }

        private void UpdateCubes()
        {
            float4 a = eightCellVertices[0];
            float4 b = eightCellVertices[1];
            float4 c = eightCellVertices[2];
            float4 d = eightCellVertices[3];
            float4 e = eightCellVertices[4];
            float4 f = eightCellVertices[5];
            float4 g = eightCellVertices[6];
            float4 h = eightCellVertices[7];
            float4 i = eightCellVertices[8];
            float4 j = eightCellVertices[9];
            float4 k = eightCellVertices[10];
            float4 l = eightCellVertices[11];
            float4 m = eightCellVertices[12];
            float4 n = eightCellVertices[13];
            float4 o = eightCellVertices[14];
            float4 p = eightCellVertices[15];

            float4[] cube1 = {a, b, c, d, e, f, g, h};
            float4[] cube2 = {a, b, c, d, e, f, g, h};
            float4[] cube3 = {a, b, c, d, e, f, g, h};
            float4[] cube4 = {a, b, c, d, e, f, g, h};
            float4[] cube5 = {a, b, c, d, e, f, g, h};
            float4[] cube6 = {a, b, c, d, e, f, g, h};
            float4[] cube7 = {a, b, c, d, e, f, g, h};
            float4[] cube8 = {a, b, c, d, e, f, g, h};

            hyperCubeXC.ForEach(q => q.planePos = planePos);
            hyperCubeXC.ForEach(q => q.planeNormal = planeNormal);
        }
    }
}