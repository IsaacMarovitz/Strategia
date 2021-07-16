using UnityEngine;
using UnityEngine.UI;

public class UIGridRenderer : Graphic {

    public Vector2Int gridSize = new Vector2Int(1, 1);
    public float borderThickness = 10f;

    float width;
    float height;
    float cellWidth;
    float cellHeight;

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        cellWidth = width / (float)gridSize.x;
        cellHeight = height / (float)gridSize.y;

        int count = 0;

        for (int y = 0; y < gridSize.y; y++) {
            for (int x = 0; x < gridSize.x; x++) {
                DrawCell(x, y, count, vh);
                count++;
            }
        }
    }

    private void DrawCell(int x, int y, int index, VertexHelper vh) {
        float distance = Mathf.Sqrt((borderThickness * borderThickness) / 2f);
        float halfDistance = distance / 2f;

        float xPos = cellWidth * x + halfDistance;
        float yPos = cellHeight * y + halfDistance;

        float leftDistance = halfDistance;
        float leftOffset = 0f;
        float rightDistance = halfDistance;
        float rightOffset = 0f;
        float topDistance = halfDistance;
        float topOffset = 0f;
        float bottomDistance = halfDistance;
        float bottomOffset = 0f;

        if (x == 0) {
            leftDistance = distance;
            leftOffset = distance / 2;
        } else {
            xPos += distance;
        }

        if (x == gridSize.x - 1) {
            rightDistance = distance;
            rightOffset = distance / 2;
        } else {
            xPos -= distance;
        }

        if (y == 0) {
            bottomDistance = distance;
            bottomOffset = distance / 2;
        } else {
            yPos += distance;
        }

        if (y == gridSize.y - 1) {
            topDistance = distance;
            topOffset = distance / 2;
        } else {
            yPos -= distance;
        }

        float leftSideOffset = leftOffset * 2 - rightOffset;
        float rightSideOffset = leftOffset - rightOffset * 2;
        float topSideOffset = bottomOffset - topOffset * 2;
        float bottomSideOffset = bottomOffset * 2 - topOffset;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        // Grid Verticies
        vertex.position = new Vector3(xPos + rightSideOffset, yPos + topSideOffset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + rightSideOffset, yPos + cellHeight + bottomSideOffset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth + leftSideOffset, yPos + cellHeight + bottomSideOffset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth + leftSideOffset, yPos + topSideOffset);
        vh.AddVert(vertex);

        // Border Verticies
        vertex.position = new Vector3(xPos + leftDistance + rightSideOffset, yPos + bottomDistance + topSideOffset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + leftDistance + rightSideOffset, yPos + (cellHeight - topDistance) + bottomSideOffset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (cellWidth - rightDistance) + leftSideOffset, yPos + (cellHeight - topDistance) + bottomSideOffset);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (cellWidth - rightDistance) + leftSideOffset, yPos + bottomDistance + topSideOffset);
        vh.AddVert(vertex);

        int offset = index * 8;

        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);

        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);

        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);

        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);
    }
}
