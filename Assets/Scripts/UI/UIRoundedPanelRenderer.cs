using UnityEngine;
using UnityEngine.UI;

public class UIRoundedPanelRenderer : Graphic {

    public float edgeThickness;
    public int numCornerVerts;

    public bool upperLeftCorner;
    public bool upperRightCorner;
    public bool lowerLeftCorner;
    public bool lowerRightCorner;

    float width;
    float height;

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        width = rectTransform.rect.width - edgeThickness;
        height = rectTransform.rect.height - edgeThickness;

        Vector2 vector;

        UIVertex vertex = new UIVertex();
        vertex.color = color;
        int index = vh.currentVertCount;

        vector = Vector2.up;

        // Main Quad
        Vector2 mainLowerLeft = new Vector2(-vector.y - width + 1, vector.x + edgeThickness);
        Vector2 mainUpperRight = new Vector2(vector.y - edgeThickness - 1, -vector.x + height);
        AddQuad(mainLowerLeft, mainUpperRight, Vector2.zero, Vector2.one, 0, vh);

        // Upper Quad
        Vector2 upperLowerLeft = new Vector2(-vector.y - width + 1, vector.x + height);
        Vector2 upperUpperRight = new Vector2(vector.y - edgeThickness - 1, vector.x + height + edgeThickness);
        AddQuad(upperLowerLeft, upperUpperRight, Vector2.zero, Vector2.one, 0, vh);

        // Lower Quad
        Vector2 lowerLowerLeft = new Vector2(-vector.y - width + 1, vector.x);
        Vector2 lowerUpperRight = new Vector2(vector.y - edgeThickness - 1, vector.x + edgeThickness);
        AddQuad(lowerLowerLeft, lowerUpperRight, Vector2.zero, Vector2.one, 0, vh);

        // Left Quad
        Vector2 leftLowerLeft = new Vector2(-vector.y - width - edgeThickness + 1, vector.x + edgeThickness);
        Vector2 leftUpperRight = new Vector2(-vector.y - width + 1, vector.x + height);
        AddQuad(leftLowerLeft, leftUpperRight, Vector2.zero, Vector2.one, 0, vh);

        // Right Quad
        Vector2 rightLowerLeft = new Vector2(vector.y - edgeThickness - 1, vector.x + edgeThickness);
        Vector2 rightUpperRight = new Vector2(vector.y - 1, vector.x + height);
        AddQuad(rightLowerLeft, rightUpperRight, Vector2.zero, Vector2.one, 0, vh);

        // Upper Left
        if (upperLeftCorner) {
            CornerCap(new Vector2(mainLowerLeft.x, upperUpperRight.y), new Vector2(leftLowerLeft.x, mainUpperRight.y), Vector2.zero, Vector2.one, 90f, false, vh);
        } else {
            AddQuad(new Vector2(mainLowerLeft.x, upperUpperRight.y), new Vector2(leftLowerLeft.x, mainUpperRight.y), Vector2.zero, Vector2.one, 0f, vh);
        }

        // Upper Right
        if (upperRightCorner) {
            CornerCap(upperUpperRight, rightUpperRight, Vector2.zero, Vector2.one, 0f, true, vh);
        } else {
            AddQuad(mainUpperRight, new Vector2(rightUpperRight.x, upperUpperRight.y), Vector2.zero, Vector2.one, 0f, vh);
        }

        // Lower Left
        if (lowerLeftCorner) {
            CornerCap(lowerLowerLeft, leftLowerLeft, Vector2.zero, Vector2.one, 180f, true, vh);
        } else {
            AddQuad(new Vector2(leftLowerLeft.x, lowerLowerLeft.y), mainLowerLeft, Vector2.zero, Vector2.one, 0f, vh);
        }

        // Lower Right
        if (lowerRightCorner) {
            CornerCap(new Vector2(lowerUpperRight.x, lowerLowerLeft.y), new Vector2(rightUpperRight.x, rightLowerLeft.y), Vector2.zero, Vector2.one, -90f, false, vh);
        } else {
            AddQuad(new Vector2(lowerUpperRight.x, lowerLowerLeft.y), new Vector2(rightUpperRight.x, rightLowerLeft.y), Vector2.zero, Vector2.one, 0f, vh);
        }
    }

    void AddQuad(Vector2 lowerLeft, Vector2 upperRight, Vector2 lowerLeftUV, Vector2 upperRightUV, float rotation, VertexHelper vh) {
        int index = vh.currentVertCount;

        vh.AddVert(lowerLeft, color, lowerLeftUV);
        vh.AddVert(new Vector2(lowerLeft.x, upperRight.y), color, Quaternion.Euler(0, 0, -rotation) * new Vector2(lowerLeftUV.x, upperRightUV.y));
        vh.AddVert(upperRight, color, upperRightUV);
        vh.AddVert(new Vector2(upperRight.x, lowerLeft.y), color, Quaternion.Euler(0, 0, rotation) * new Vector2(upperRightUV.x, lowerLeftUV.y));

        vh.AddTriangle(index + 0, index + 1, index + 2);
        vh.AddTriangle(index + 0, index + 2, index + 3);
    }

    void CornerCap (Vector2 lowerLeft, Vector2 upperRight, Vector2 lowerLeftUV, Vector2 upperRightUV, float angleOffset, bool flip, VertexHelper vh) {
        float angleStep = 90f / (float)numCornerVerts;
        int index = vh.currentVertCount;
        Vector2 center = new Vector2(lowerLeft.x, upperRight.y);

        Vector2[] capVerts = new Vector2[numCornerVerts];
        for (int i = 0; i < numCornerVerts; i++) {
            Vector2 vertex = AngleToPos(angleStep * i + angleOffset);
            vertex += center;
            capVerts[i] = vertex;
        }

        if (numCornerVerts == 0) {
            vh.AddVert(lowerLeft, color, lowerLeftUV);
            vh.AddVert(new Vector2(lowerLeft.x, upperRight.y), color, new Vector2(lowerLeftUV.x, upperRightUV.y));
            vh.AddVert(upperRight, color, upperRightUV);
            vh.AddTriangle(index + 0, index + 1, index + 2);
        } else {
            for (int i = 0; i < numCornerVerts; i++) {
                vh.AddVert(capVerts[i], color, Vector2.zero);
                vh.AddVert(new Vector2(lowerLeft.x, upperRight.y), color, new Vector2(lowerLeftUV.x, upperRightUV.y));
                
                if (i == numCornerVerts - 1) {
                    if (flip) {
                        vh.AddVert(lowerLeft, color, lowerLeftUV);
                    } else {
                        vh.AddVert(upperRight, color, upperRightUV);
                    }
                } else {
                    vh.AddVert(capVerts[i + 1], color, Vector2.zero);
                }

                vh.AddTriangle(index + 0, index + 1, index + 2);
                index += 3;
            }
        }
    }

    Vector3 AngleToPos(float angle) {
        float x = edgeThickness * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = edgeThickness * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }
}