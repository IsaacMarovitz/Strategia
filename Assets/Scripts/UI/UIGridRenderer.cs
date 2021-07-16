using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectMask2D))]
public class UIGridRenderer : Graphic {

    public Vector2Int gridSize = new Vector2Int(1, 1);
    public float columnThickness = 10f;
    public float rowThickness = 10f;
    public float edgeThickness = 10f;
    public bool maskEdge = true;
    public Color edgeColor;
    public TMP_Text textPrefab;

    float width;
    float height;
    float cellWidth;
    float cellHeight;

    private RectMask2D rectMask2D;

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        width = rectTransform.rect.width - (edgeThickness * 4);
        height = rectTransform.rect.height - (edgeThickness * 4);

        cellWidth = width / (float)gridSize.x;
        cellHeight = height / (float)gridSize.y;

        int count = 0;

        for (int y = 0; y < gridSize.y; y++) {
            if (y == 0) { continue; }
            DrawRow(y, count, vh);
            count++;
            for (int x = 0; x < gridSize.x; x++) {
                if (x == 0) { continue; }
                DrawColumn(x, count, vh);
                count++;
            }
        }

        DrawEdge(count, vh);
    }

    private void DrawColumn(int x, int index, VertexHelper vh) {
        Vector2 vector = Vector2.up;

        UIVertex vertex = new UIVertex();
        vertex.color = color;

        vertex.position = new Vector3(-vector.y, vector.x) * columnThickness;
        vertex.position += new Vector3(cellWidth * x, 0);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * columnThickness;
        vertex.position += new Vector3(cellWidth * x, 0);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * columnThickness;
        vertex.position += new Vector3(cellWidth * x, height);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * columnThickness;
        vertex.position += new Vector3(cellWidth * x, height);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        int offset = index * 4;
        vh.AddTriangle(offset + 2, offset + 1, offset + 0);
        vh.AddTriangle(offset + 1, offset + 2, offset + 3);
    }

    private void DrawRow(int y, int index, VertexHelper vh) {
        Vector2 vector = Vector2.right;

        UIVertex vertex = new UIVertex();
        vertex.color = color;

        vertex.position = new Vector3(-vector.y, vector.x) * rowThickness;
        vertex.position += new Vector3(0, cellHeight * y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * rowThickness;
        vertex.position += new Vector3(0, cellHeight * y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * rowThickness;
        vertex.position += new Vector3(width, cellHeight * y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * rowThickness;
        vertex.position += new Vector3(width, cellHeight * y);
        vertex.position += new Vector3(edgeThickness * 2, edgeThickness * 2);
        vh.AddVert(vertex);

        int offset = index * 4;
        vh.AddTriangle(offset + 2, offset + 1, offset + 0);
        vh.AddTriangle(offset + 1, offset + 2, offset + 3);
    }

    private void DrawEdge(int index, VertexHelper vh) {
        Vector2 vector = Vector2.up;

        UIVertex vertex = new UIVertex();
        vertex.color = edgeColor;

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(edgeThickness, 0);
        vertex.uv0 = Vector2.zero;
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(0, rectTransform.rect.height);
        vertex.position += new Vector3(edgeThickness, 0);
        vertex.uv0 = Vector2.up;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(0, rectTransform.rect.height);
        vertex.position += new Vector3(edgeThickness, 0);
        vertex.uv0 = Vector2.one;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(edgeThickness, 0);
        vertex.uv0 = Vector2.right;
        vh.AddVert(vertex);

        int offset = index * 4;
        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 0, offset + 2, offset + 3);

        index++;

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, 0);
        vertex.position += new Vector3(-edgeThickness, 0);
        vertex.uv0 = Vector2.zero;
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, rectTransform.rect.height);
        vertex.position += new Vector3(-edgeThickness, 0);
        vertex.uv0 = Vector2.up;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, rectTransform.rect.height);
        vertex.position += new Vector3(-edgeThickness, 0);
        vertex.uv0 = Vector2.one;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, 0);
        vertex.position += new Vector3(-edgeThickness, 0);
        vertex.uv0 = Vector2.right;
        vh.AddVert(vertex);

        offset = index * 4;
        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 0, offset + 2, offset + 3);

        index++;
        vector = Vector2.right;

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(0, rectTransform.rect.height);
        vertex.position += new Vector3(0, -edgeThickness);
        vertex.uv0 = Vector2.zero;
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, rectTransform.rect.height);
        vertex.position += new Vector3(0, -edgeThickness);
        vertex.uv0 = Vector2.up;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, rectTransform.rect.height);
        vertex.position += new Vector3(0, -edgeThickness);
        vertex.uv0 = Vector2.one;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(0, rectTransform.rect.height);
        vertex.position += new Vector3(0, -edgeThickness);
        vertex.uv0 = Vector2.right;
        vh.AddVert(vertex);

        offset = index * 4;
        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 0, offset + 2, offset + 3);

        index++;

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(0, 0);
        vertex.position += new Vector3(0, edgeThickness);
        vertex.uv0 = Vector2.zero;
        vh.AddVert(vertex);

        vertex.position = new Vector3(-vector.y, vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, 0);
        vertex.position += new Vector3(0, edgeThickness);
        vertex.uv0 = Vector2.up;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(rectTransform.rect.width, 0);
        vertex.position += new Vector3(0, edgeThickness);
        vertex.uv0 = Vector2.one;
        vh.AddVert(vertex);

        vertex.position = new Vector3(vector.y, -vector.x) * edgeThickness;
        vertex.position += new Vector3(0, 0);
        vertex.position += new Vector3(0, edgeThickness);
        vertex.uv0 = Vector2.right;
        vh.AddVert(vertex);

        offset = index * 4;
        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 0, offset + 2, offset + 3);
    }

    private void Update() {
        if (rectMask2D != null) {
            if (maskEdge) {
                Vector4 padding = new Vector4(edgeThickness * 2, edgeThickness * 2, edgeThickness * 2, edgeThickness * 2);
                if (rectMask2D.padding != padding) {
                    rectMask2D.padding = padding;
                }
            } else {
                if (rectMask2D.padding != Vector4.zero) {
                    rectMask2D.padding = Vector4.zero;
                }
            }
        } else {
            rectMask2D = GetComponent<RectMask2D>();
        }
    }
}
