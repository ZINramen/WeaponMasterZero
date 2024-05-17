using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
[ExecuteInEditMode]
public class UICircle : Graphic
{
    [SerializeField]
    Texture m_Texture;
    public int sides = 20;
    public Color circleColor = Color.white;
    public int rotation = 0;
    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }
    /// <summary>
    /// Texture to be used.
    /// </summary>
    public Texture texture
    {
        get
        {
            return m_Texture;
        }
        set
        {
            if (m_Texture == value)
                return;
            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }
    protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
    {
        UIVertex[] vbo = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = circleColor;
            vert.position = vertices[i];
            vert.uv0 = uvs[i];
            vbo[i] = vert;
        }
        return vbo;
    }
    //    protected override void OnFillVBO(List<UIVertex> vbo)
    protected override void OnPopulateMesh(VertexHelper vbo)
    {
        float outer = -rectTransform.pivot.x * rectTransform.rect.width;
        float inner = 0;
        //        vbo.Clear();
        vbo.Clear();
        UIVertex vert = UIVertex.simpleVert;
        Vector2 prevX = Vector2.zero;
        Vector2 prevY = Vector2.zero;
        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(0, 1);
        Vector2 uv2 = new Vector2(1, 1);
        Vector2 uv3 = new Vector2(1, 0);
        Vector2 pos0;
        Vector2 pos1;
        Vector2 pos2;
        Vector2 pos3;
        float degrees = 360f / sides;
        for (int i = 0; i < sides + 1; i++)
        {
            float rad = Mathf.Deg2Rad * (i * degrees - 90 + rotation);
            float c = Mathf.Cos(rad);
            float s = Mathf.Sin(rad);
            float x = outer * c;
            float y = inner * c;
            uv0 = new Vector2(0, 1);
            uv1 = new Vector2(1, 1);
            uv2 = new Vector2(1, 0);
            uv3 = new Vector2(0, 0);
            pos0 = prevX;
            pos1 = new Vector2(outer * c, outer * s);
            pos2 = Vector2.zero;
            pos3 = Vector2.zero;
            prevX = pos1;
            prevY = pos2;
            vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
        }
    }
}