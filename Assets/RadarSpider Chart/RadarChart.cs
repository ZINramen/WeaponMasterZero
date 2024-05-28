using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class GraphData
    {
        [Header("Values and Display")]
        [SerializeField]
        [Range(0, 1)]
        public List<float> values = new List<float>();
        [SerializeField]
        public Color color = new Color(1, 0, 0, .5f);
        [SerializeField]
        public bool fill = true;
        [SerializeField]
        [Range(0.0f, 1.0f)]
        public float thickness = 0.1f;

        //Data Points
        [Header("Data Points")]
        [SerializeField]
        public bool dataPoints = false;
        [SerializeField]
        [Range(0f, 1f)]
        public float dataPointsDiameter = 0.04f;
        [SerializeField]
        [Range(3, 50)]
        public int dataPointsSides = 10;
        [SerializeField]
        [Range(0, 360)]
        public int dataPointsRotation = 0;
        [SerializeField]
        public Color dataPointsColor = new Color(1,0,0,1);
    }

    [AddComponentMenu("UI/Radar Chart")]
    public class RadarChart : MaskableGraphic
    {
        //Global settings
        [Header("General Settings")]
        [SerializeField]
        [Range(3, 30)]
        public int sides = 3;
        [SerializeField]
        [Range(0, 360)]
        public float rotation = 0;
        [SerializeField]
        public Color background = Color.white;

        //Data to display
        [Header("Chart Data")]
        [SerializeField]
        public List<GraphData> data = new List<GraphData>();

        //Vertex Text
        [Header("Vertex Labels")]
        [SerializeField]
        public bool vertexLabels = true;
        [SerializeField]
        [Range(20, 1000)]
        public float vertexLabelBoxSize = 100;
        [SerializeField]
        [Range(7, 100)]
        public int vertexLabelFontSize = 18;
        [SerializeField]
        public Font vertexLabelFont;
        [SerializeField]
        [Range(0f, 2f)]
        public float vertexLabelPosition = 1.05f;
        [SerializeField]
        public string[] vertexLabelValues = new string[1];

        //Horizontal Lines
        [Header("Horizontal Lines")]
        [SerializeField]
        public bool horizontalLines = true;
        [SerializeField]
        [Range(2, 100)]
        public int horizontalDivisions = 10;
        [SerializeField]
        [Range(0f, 0.1f)]
        public float horizontalLineThickness = 0.01f;
        [SerializeField]
        public Color horizontalLineColor = Color.black;

        //Horizontal Divisions
        [Header("Regions")]
        [SerializeField]
        public bool regions = false;
        public Color regionsInnerColor = new Color(0.7176471f, 0.01898048f, 0.003921578f, 1);
        public Color regionsOuterColor = new Color(0, 0.6588235f, 0.003921569f, 1);

        //Vertical Lines
        [Header("Vertical Lines")]
        [SerializeField]
        public bool verticalLines = true;
        [SerializeField]
        [Range(0f, 1f)]
        public float verticalLineThickness = 0.02f;
        [SerializeField]
        public Color verticalLineColor = Color.black;

        //Outer Border
        [Header("Border")]
        public bool border = true;
        public Color borderColor = Color.black;
        [Range(0f, 1f)]
        public float borderThickness = 0.01f;

        private float size = 0;
        private bool recalculate = false;

        private Vector2 Rotate(Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        //Start of Editor
        new void Start()
        {
            base.Start();
            recalculate = true;
            Update();
        }

        void Update()
        {
            //Do things that cant be done inside Graphics call (OnPopulateMesh)
            if (recalculate)
            {
                recalculate = false;

                //Delete previous Vertices Text
                if (gameObject.transform.Find("VertexLabels") != null)
                    DestroyImmediate(gameObject.transform.Find("VertexLabels").gameObject);

                //Load default font
                if (vertexLabelFont == null)
                    vertexLabelFont = Font.CreateDynamicFontFromOSFont("Arial", 14);

                //Vertices Text label
                if (vertexLabels)
                {
                    //Holder
                    GameObject verticesText = new GameObject("VertexLabels");
                    verticesText.transform.SetParent(gameObject.transform);
                    verticesText.transform.localPosition = Vector3.zero;
                    verticesText.transform.localScale = Vector3.one;
                    //Degree step
                    float degreeStep = 360f / sides;
                    //Resize if needed
                    if (vertexLabelValues.Length != sides)
                    {
                        vertexLabelValues = new string[sides];
                        for (int i = 0; i < sides; i++) vertexLabelValues[i] = "TXT";
                    }
                    for (int i = 0; i < sides; i++)
                    {
                        //Create
                        GameObject vertex = new GameObject("Vertex Text " + i);
                        RectTransform transf = vertex.AddComponent<RectTransform>();
                        transf.sizeDelta = new Vector2(vertexLabelBoxSize, vertexLabelBoxSize);

                        //Add Text
                        Text text = vertex.AddComponent<Text>();
                        text.text = vertexLabelValues[i];
                        text.fontSize = vertexLabelFontSize;
                        text.font = vertexLabelFont;

                        //Prepare Parent
                        vertex.transform.SetParent(verticesText.transform);
                        vertex.transform.localPosition = Vector3.zero;
                        vertex.transform.localScale = Vector3.one;
                        vertex.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

                        //Position and Text Alignment
                        float deg = i * degreeStep + rotation + 90;
                        if (deg > 360)
                            deg = deg % 360;
                        float rad = Mathf.Deg2Rad * deg;
                        float c = Mathf.Cos(rad);
                        float s = Mathf.Sin(rad);
                        vertex.transform.localPosition = new Vector2(c * size / 2 * vertexLabelPosition, s * size / 2 * vertexLabelPosition);
                        float clampTolerance = 12.5f;
                        //Clamp to square angles
                        if (Mathf.Abs(deg - 0) < clampTolerance || Mathf.Abs(deg - 360) < clampTolerance)
                        {
                            text.alignment = TextAnchor.MiddleLeft;
                            vertex.transform.localPosition += new Vector3(0, vertexLabelBoxSize / 2, 0);
                        }
                        else if (Mathf.Abs(deg - 90) < clampTolerance)
                        {
                            text.alignment = TextAnchor.LowerCenter;
                            vertex.transform.localPosition += new Vector3(-vertexLabelBoxSize / 2, vertexLabelBoxSize, 0);
                        }
                        else if (Mathf.Abs(deg - 180) < clampTolerance)
                        {
                            text.alignment = TextAnchor.MiddleRight;
                            vertex.transform.localPosition += new Vector3(-vertexLabelBoxSize, vertexLabelBoxSize / 2, 0);
                        }
                        else if (Mathf.Abs(deg - 270) < clampTolerance)
                        {
                            text.alignment = TextAnchor.UpperCenter;
                            vertex.transform.localPosition += new Vector3(-vertexLabelBoxSize / 2, 0, 0);
                        }
                        //Quadrants
                        else if (deg > 0 && deg < 90)
                        {
                            text.alignment = TextAnchor.LowerLeft;
                            vertex.transform.localPosition += new Vector3(0, vertexLabelBoxSize, 0);
                        }
                        else if (deg > 90 && deg < 180)
                        {
                            text.alignment = TextAnchor.LowerRight;
                            vertex.transform.localPosition += new Vector3(-vertexLabelBoxSize, vertexLabelBoxSize, 0);
                        }
                        else if (deg > 180 && deg < 270)
                        {
                            text.alignment = TextAnchor.UpperRight;
                            vertex.transform.localPosition += new Vector3(-vertexLabelBoxSize, 0, 0);
                        }
                        else if (deg > 270 && deg < 360)
                        {
                            text.alignment = TextAnchor.UpperLeft;
                            vertex.transform.localPosition += new Vector3(0, 0, 0);
                        }
                    }
                }

                //Data Points
                for (int d = 0; d < data.Count; d++)
                {
                    //Delete previous Data Points
                    if (gameObject.transform.Find("DataPoints " + d) != null)
                        DestroyImmediate(gameObject.transform.Find("DataPoints " + d).gameObject);

                    //Data Points
                    if (data[d].dataPoints)
                    {
                        //Holder
                        GameObject dataPoints = new GameObject("DataPoints " + d);
                        dataPoints.transform.SetParent(gameObject.transform);
                        dataPoints.transform.localPosition = Vector3.zero;
                        dataPoints.transform.localScale = Vector3.one;
                        //Degree step
                        float degreeStep = 360f / sides;
                        for (int i = 0; i < sides; i++)
                        {
                            //Create
                            GameObject vertex = new GameObject("Data Point " + i);
                            vertex.AddComponent<CanvasRenderer>();
                            RectTransform transf = vertex.AddComponent<RectTransform>();
                            UICircle point = vertex.AddComponent<UICircle>();
                            point.circleColor = data[d].dataPointsColor;
                            point.sides = data[d].dataPointsSides;
                            point.rotation = data[d].dataPointsRotation;
                            transf.sizeDelta = new Vector2(size / 2 * data[d].dataPointsDiameter, size / 2 * data[d].dataPointsDiameter);
                            transf.pivot = new Vector2(.5f, .5f);

                            //Prepare Parent
                            vertex.transform.SetParent(dataPoints.transform);
                            vertex.transform.localPosition = Vector3.zero;
                            vertex.transform.localScale = Vector3.one;

                            //Position and Text Alignment
                            float deg = i * degreeStep + rotation + 90;
                            if (deg > 360)
                                deg = deg % 360;
                            float rad = Mathf.Deg2Rad * deg;
                            float c = Mathf.Cos(rad);
                            float s = Mathf.Sin(rad);
                            vertex.transform.localPosition = new Vector2(c * size / 2 * data[d].values[i], s * size / 2 * data[d].values[i]);
                        }
                    }
                }
            }
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs, Color color)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            recalculate = true;

            //Default Data
            if (data.Count == 0)
            {
                GraphData n = new GraphData();
                data.Add(n);
            }

            //Reset vertice structures if new sides
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].values.Count != sides)
                {
                    data[i].values = new List<float>();
                    for (int j = 0; j < sides; j++) data[i].values.Add(Random.Range(.25f, 1f));
                }
            }
            if (vertexLabelValues.Length != sides)
            {
                vertexLabelValues = new string[sides];
                for (int i = 0; i < sides; i++) vertexLabelValues[i] = "TXT";
            }

            //Recalculate size
            size = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);

            //Rebuild Mesh
            vh.Clear();

            //UVs
            Vector2 uv0 = new Vector2(0, 1);
            Vector2 uv1 = new Vector2(1, 1);
            Vector2 uv2 = new Vector2(1, 0);
            Vector2 uv3 = new Vector2(0, 0);

            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;

            float degrees = 360f / sides;

            //Calculate normalized vector for each vertex
            Vector2[] norms = new Vector2[sides];
            for (int i = 0; i < sides; i++)
            {
                float rad = Mathf.Deg2Rad * (90 + i * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                norms[i] = new Vector2(c, s);
            }

            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;

            //Background Color
            if (background.a > 0)
                for (int i = 0; i < sides + 1; i++)
                {
                    //Distance
                    float dist = 1;

                    //Normalized Vector
                    Vector2 norm = Vector2.zero;
                    if (i == sides)
                        norm = norms[0];
                    else
                        norm = norms[i];

                    //Line Quad
                    float outer = rectTransform.pivot.x * size * dist;
                    pos0 = prevX;
                    pos1 = norm * outer;
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                    prevX = pos1;
                    prevY = pos2;

                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, background));
                }

            //Division Colors
            if (regions)
            {
                //HSV defined colors
                float h1;
                float s1;
                float v1;
                float a1 = regionsInnerColor.a;
                Color.RGBToHSV(regionsInnerColor, out h1, out s1, out v1);
                float h2;
                float s2;
                float v2;
                float a2 = regionsOuterColor.a;
                Color.RGBToHSV(regionsOuterColor, out h2, out s2, out v2);

                for (int d = 0; d < horizontalDivisions; d++)
                {
                    //Distance
                    float dist = (float)(d + 1) / horizontalDivisions;

                    //Lerped Hue
                    float lerp = (float)(d) / (horizontalDivisions - 1);
                    float hue = Mathf.Lerp(h1, h2, lerp);
                    float saturation = Mathf.Lerp(s1, s2, lerp);
                    float value = Mathf.Lerp(v1, v2, lerp);
                    float alpha = Mathf.Lerp(a1, a2, lerp);

                    Color color = Color.HSVToRGB(hue, saturation, value);
                    color.a = alpha;

                    for (int i = 0; i < sides + 1; i++)
                    {

                        //Normalized Vector
                        Vector2 norm = Vector2.zero;
                        if (i == sides)
                            norm = norms[0];
                        else
                            norm = norms[i];

                        //Line Quad
                        float outer = rectTransform.pivot.x * size * dist;
                        float inner = rectTransform.pivot.x * size * dist - Mathf.Clamp(1f / horizontalDivisions * size / 2, 0, dist * size / 2); //Clamp prevent thickness passing to opposite side
                        pos0 = prevX;
                        pos1 = norm * outer;

                        //Non Fill
                        pos2 = norm * inner;
                        pos3 = prevY;

                        prevX = pos1;
                        prevY = pos2;

                        vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, color));
                    }
                }
            }

            //Adjusted horizontal thickness to consider angle of sides (original thickness is at the axis, with small amount of sides actual thickness gets cut)
            float adjustedHorizontalThickness = horizontalLineThickness / Mathf.Cos(Mathf.Deg2Rad * degrees / 2);

            //Horizontal Metric Lines
            prevX = Vector2.zero;
            prevY = Vector2.zero;
            if (horizontalLines)
                for (int d = 0; d < horizontalDivisions; d++)
                {
                    for (int i = 0; i < sides + 1; i++)
                    {
                        //Distance
                        float dist = (float)(d + 1) / horizontalDivisions;

                        //Normalized Vector
                        Vector2 norm = Vector2.zero;
                        if (i == sides)
                            norm = norms[0];
                        else
                            norm = norms[i];

                        //Line Quad
                        float outer = rectTransform.pivot.x * size * dist;
                        float inner = rectTransform.pivot.x * size * dist - Mathf.Clamp(adjustedHorizontalThickness * size / 2, 0, dist * size / 2); //Clamp prevent thickness passing to opposite side
                        pos0 = prevX;
                        pos1 = norm * outer;

                        //Non Fill
                        pos2 = norm * inner;
                        pos3 = prevY;

                        prevX = pos1;
                        prevY = pos2;

                        vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, horizontalLineColor));
                    }
                }

            //Vertical Metric Lines
            if (verticalLines)
                for (int i = 0; i < sides; i++)
                {
                    //Distance cutting excess from thickness
                    float vertexHalfAngle = 180 - 90 - degrees / 2;
                    float excess = verticalLineThickness / 2 / Mathf.Tan(Mathf.Deg2Rad * vertexHalfAngle);
                    float dist = 1 - excess;

                    //Normalized Vector
                    Vector2 norm = norms[i];

                    //Rotated Vectors
                    Vector2 right = Rotate(norm, 90);
                    Vector2 left = Rotate(norm, 270);

                    //Main part
                    pos0 = Vector2.zero + right * verticalLineThickness / 2 * size / 2;
                    pos1 = Vector2.zero + left * verticalLineThickness / 2 * size / 2;
                    pos3 = norm * size / 2 * dist + right * verticalLineThickness / 2 * size / 2;
                    pos2 = norm * size / 2 * dist + left * verticalLineThickness / 2 * size / 2;
                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, verticalLineColor));

                    //Cap
                    pos0 = norm * size / 2;
                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos0, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, verticalLineColor));
                }

            //GraphData graphs (each with color, fill and thickness)
            for (int d = 0; d < data.Count; d++)
            {
                prevX = Vector2.zero;
                prevY = Vector2.zero;
                Vector2 prevPivotLeft = Vector2.zero;
                Vector2 prevPivotRight = Vector2.zero;
                for (int i = 0; i < sides + 1; i++)
                {
                    //Distance
                    float dist = 0;
                    if (i == sides)
                        dist = data[d].values[0];
                    else
                        dist = data[d].values[i];

                    //Normalized Vector
                    Vector2 norm = Vector2.zero;
                    if (i == sides)
                        norm = norms[0];
                    else
                        norm = norms[i];

                    //Line Quad
                    float outer = rectTransform.pivot.x * size * dist;
                    pos0 = prevX;
                    pos1 = norm * outer;
                    if (data[d].fill)
                    {
                        pos2 = Vector2.zero;
                        pos3 = Vector2.zero;
                    }
                    else
                    {
                        Vector2 lineVec = (GetCycled(norms, i + 1) * rectTransform.pivot.x * size * GetCycled(data[d].values, i + 1)) - (GetCycled(norms, i) * rectTransform.pivot.x * size * GetCycled(data[d].values, i));
                        lineVec = lineVec.normalized;

                        Vector2 right = Rotate(lineVec, 90);
                        Vector2 left = Rotate(lineVec, 270);

                        pos0 = GetCycled(norms, i) * rectTransform.pivot.x * size * GetCycled(data[d].values, i) + right * data[d].thickness / 2 * size / 2;
                        pos1 = GetCycled(norms, i) * rectTransform.pivot.x * size * GetCycled(data[d].values, i) + left * data[d].thickness / 2 * size / 2;

                        pos2 = GetCycled(norms, i + 1) * rectTransform.pivot.x * size * GetCycled(data[d].values, i + 1) + left * data[d].thickness / 2 * size / 2;
                        pos3 = GetCycled(norms, i + 1) * rectTransform.pivot.x * size * GetCycled(data[d].values, i + 1) + right * data[d].thickness / 2 * size / 2;
                    }

                    prevX = pos1;
                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, data[d].color));

                    //Connection Cap
                    if (prevPivotLeft.x != 0)
                    {
                        vh.AddUIVertexQuad(SetVbo(new[] { pos1, pos1, GetCycled(norms, i) * rectTransform.pivot.x * size * GetCycled(data[d].values, i), prevPivotLeft }, new[] { uv0, uv1, uv2, uv3 }, data[d].color));
                    }
                    prevPivotLeft = pos2;
                    if (prevPivotRight.x != 0)
                    {
                        vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos0, GetCycled(norms, i) * rectTransform.pivot.x * size * GetCycled(data[d].values, i), prevPivotRight }, new[] { uv0, uv1, uv2, uv3 }, data[d].color));
                    }
                    prevPivotRight = pos3;
                }
            }

            //Border around
            float adjustedBorderThickness = borderThickness / Mathf.Cos(Mathf.Deg2Rad * degrees / 2);
            if (border)
            {
                prevX = Vector2.zero;
                prevY = Vector2.zero;
                for (int i = 0; i < sides + 1; i++)
                {
                    //Distance
                    float dist = 1;

                    //Normalized Vector
                    Vector2 norm = Vector2.zero;
                    if (i == sides)
                        norm = norms[0];
                    else
                        norm = norms[i];

                    //Line Quad
                    float outer = rectTransform.pivot.x * size * dist;
                    float inner = rectTransform.pivot.x * size * dist + Mathf.Clamp(adjustedBorderThickness * size / 2, 0, dist * size / 2); //Clamp prevent thickness passing to opposite side
                    pos0 = prevX;
                    pos1 = norm * outer;

                    //Non Fill
                    pos2 = norm * inner;
                    pos3 = prevY;

                    prevX = pos1;
                    prevY = pos2;

                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, borderColor));
                }
            }
        }

        public Vector2 GetCycled(Vector2[] list, int id)
        {
            return list[id % list.Length];
        }
        public float GetCycled(List<float> list, int id)
        {
            return list[id % list.Count];
        }
    }
}