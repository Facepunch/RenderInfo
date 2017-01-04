using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct RendererInstance
{
    public bool IsVisible { get; set; }
    public bool CastShadows { get; set; }
    public bool Enabled { get; set; }
    public bool RecieveShadows { get; set; }
    public string Center { get; set; }
    public string Size { get; set; }
    public float Distance { get; set; }
    public int BoneCount { get; set; }
    public int MaterialCount { get; set; }
    public int VertexCount { get; set; }
    public int TriangleCount { get; set; }
    public int SubMeshCount { get; set; }
    public int BlendShapeCount { get; set; }
    public string RenderType { get; set; }
    public string MeshName { get; set; }
    public bool UpdateWhenOffscreen { get; set; }
    public string SkinQuality { get; set; }
}