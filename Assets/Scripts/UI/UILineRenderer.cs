using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineRenderer : Graphic {

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

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

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
        UIVertex[] vertices = new UIVertex[4];
        Vector2 vector = b - a;
        vector.Normalize();

        vertices[0].color = color;
        vertices[0].position = new Vector3(-vector.y, vector.x) * thickness;
        vertices[0].position += new Vector3(unitWidth * a.x, unitHeight * a.y);

        vertices[1].color = color;
        vertices[1].position = new Vector3(vector.y, -vector.x) * thickness;
        vertices[1].position += new Vector3(unitWidth * a.x, unitHeight * a.y);

        vertices[2].color = color;
        vertices[2].position = new Vector3(-vector.y, vector.x) * thickness;
        vertices[2].position += new Vector3(unitWidth * b.x, unitHeight * b.y);

        vertices[3].color = color;
        vertices[3].position = new Vector3(vector.y, -vector.x) * thickness;
        vertices[3].position += new Vector3(unitWidth * b.x, unitHeight * b.y);

        vh.AddVert(vertices[0]);
        vh.AddVert(vertices[1]);
        vh.AddVert(vertices[2]);
        vh.AddVert(vertices[3]);
    }

    void CornerCap(Vector2 point, VertexHelper vh) {
        float angleStep = 360f / (float)numCornerVerts;
        int index = vh.currentVertCount;

        UIVertex vertex = new UIVertex();
        vertex.color = capColor;
        vertex.position = new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        for (int i = 0; i < numCornerVerts; i++) {
            vertex.position = AngleToPos(angleStep * i);
            vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
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
            }
        }
    }
}