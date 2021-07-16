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
    //public Sprite sprite;

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

    /*protected override void UpdateMaterial() {
        base.UpdateMaterial();
        if (sprite == null) {
           canvasRenderer.SetTexture(null);
        } else {
           canvasRenderer.SetTexture(sprite.texture);
        }
    }*/

    private void DrawColumn(int x, int index, VertexHelper vh) {
        Vector2 vector = Vector2.up;
        vector *= columnThickness;
        float xOffset = cellWidth * x;
        Vector2 thicknessOffset = new Vector2(edgeThickness * 2, edgeThickness * 2);

        Vector2 lowerLeft = new Vector2(-vector.y + xOffset, vector.x);
        lowerLeft += thicknessOffset;

        Vector2 upperRight = new Vector2(vector.y + xOffset, -vector.x + height);
        upperRight += thicknessOffset;

        AddQuad(lowerLeft, upperRight, color, Vector2.zero, Vector2.one, 0, vh);
    }

    private void DrawRow(int y, int index, VertexHelper vh) {
        Vector2 vector = Vector2.right;
        vector *= rowThickness;
        float yOffset = cellHeight * y;
        Vector2 thicknessOffset = new Vector2(edgeThickness * 2, edgeThickness * 2);

        Vector2 lowerLeft = new Vector2(vector.y, -vector.x + yOffset);
        lowerLeft += thicknessOffset;

        Vector2 upperRight = new Vector2(-vector.y + width, vector.x + yOffset);
        upperRight += thicknessOffset;

        AddQuad(lowerLeft, upperRight, color, Vector2.zero, Vector2.one, 90, vh);
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

    void AddQuad(Vector2 lowerLeft, Vector2 upperRight, Color color, Vector2 lowerLeftUV, Vector2 upperRightUV, float rotation, VertexHelper vh) {
        UIVertex vertex = new UIVertex();
        vertex.color = edgeColor;
        int index = vh.currentVertCount;

        vh.AddVert(lowerLeft, color, lowerLeftUV);
        vh.AddVert(new Vector2(lowerLeft.x, upperRight.y), color, Quaternion.Euler(0, 0, -rotation) * new Vector2(lowerLeftUV.x, upperRightUV.y));
        vh.AddVert(upperRight, color, upperRightUV);
        vh.AddVert(new Vector2(upperRight.x, lowerLeft.y), color, Quaternion.Euler(0, 0, rotation) * new Vector2(upperRightUV.x, lowerLeftUV.y));

        vh.AddTriangle(index + 0, index + 1, index + 2);
        vh.AddTriangle(index + 0, index + 2, index + 3);
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
