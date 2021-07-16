using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineRenderer : MaskableGraphic {

    public Vector2Int gridSize;
    public UIGridRenderer grid;
    public List<Vector2> points = new List<Vector2>();

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public float thickness = 10f;
    public float capThickness = 10f;
    public Color capColor;
    public int numCornerVerts = 0;

    private float edgeThickness;

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        if (grid == null) { return; }

        gridSize = grid.gridSize;
        edgeThickness = grid.edgeThickness;

        width = rectTransform.rect.width - (edgeThickness * 4);
        height = rectTransform.rect.height - (edgeThickness * 4);

        unitWidth = width / (float)gridSize.x;
        unitHeight = height / (float)gridSize.y;

        if (points.Count < 2) {
            return;
        }

        for (int i = 0; i < points.Count - 1; i++) {
            DrawVerticesForPoints(points[i], points[i + 1], vh);
        }

        int cornerVerts = 0;

        for (int i = 0; i < points.Count - 1; i++) {
            int index = i * 4 + cornerVerts;
            vh.AddTriangle(index + 0, index + 2, index + 1);
            vh.AddTriangle(index + 1, index + 3, index + 2);
        }

        for (int i = 0; i < points.Count; i++) {
            CornerCap(points[i], vh);
        }
    }

    void DrawVerticesForPoints(Vector2 a, Vector2 b, VertexHelper vh) {
        Vector2 vector = b - a;
        vector.Normalize();

        UIVertex vertex = new UIVertex();
        vertex.color = color;

        vertex.position = new Vector3(-vector.y, vector.x) * thickness;
        vertex.position += new Vector3(unitWidth * a.x, unitHeight * a.y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * thickness;
        vertex.position += new Vector3(unitWidth * a.x, unitHeight * a.y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * thickness;
        vertex.position += new Vector3(unitWidth * b.x, unitHeight * b.y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * thickness;
        vertex.position += new Vector3(unitWidth * b.x, unitHeight * b.y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);
    }

    void CornerCap(Vector2 point, VertexHelper vh) {
        float angleStep = 360f / (float)numCornerVerts;
        int index = vh.currentVertCount;

        UIVertex vertex = new UIVertex();
        vertex.color = capColor;
        vertex.position = new Vector3(unitWidth * point.x, unitHeight * point.y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        for (int i = 0; i < numCornerVerts; i++) {
            vertex.position = AngleToPos(angleStep * i);
            vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
            vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
            vh.AddVert(vertex);
        }

        for (int i = 0; i < numCornerVerts; i++) {
            if (i == 0) {
                vh.AddTriangle(index, index + 1, index + numCornerVerts);
            } else {
                vh.AddTriangle(index, index + i + 1, index + i);
            }
        }
    }

    Vector3 AngleToPos(float angle) {
        float x = capThickness * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = capThickness * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }

    private void Update() {
        if (grid != null) {
            if (gridSize != grid.gridSize) {
                gridSize = grid.gridSize;
                SetAllDirty();
            }

            if (edgeThickness != grid.edgeThickness) {
                edgeThickness = grid.edgeThickness;
                SetAllDirty();
            }
        }
    }
}