using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float gridSize = 1.0f;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public Vector3 initialBuildingPosition;

    public Vector3 GetGridPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt((position.x - initialBuildingPosition.x) / gridSize);
        int y = Mathf.RoundToInt((position.y - initialBuildingPosition.y) / gridSize);

        Vector3 gridPosition = new Vector3(x * gridSize, y * gridSize, 0) + initialBuildingPosition;
        return gridPosition;
    }

    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        // Convert world position to grid coordinates
        int gridX = Mathf.RoundToInt(worldPosition.x / gridSize);
        int gridY = Mathf.RoundToInt(worldPosition.y / gridSize);

        // Convert grid coordinates back to world position
        float snappedX = gridX * gridSize;
        float snappedY = gridY * gridSize;

        // This is the snapped world position
        return new Vector3(snappedX, snappedY, worldPosition.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int x = 0; x <= gridWidth; x++)
        {
            Gizmos.DrawLine(new Vector3(x * gridSize, 0, 0), new Vector3(x * gridSize, gridHeight * gridSize, 0));
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Gizmos.DrawLine(new Vector3(0, y * gridSize, 0), new Vector3(gridWidth * gridSize, y * gridSize, 0));
        }
    }
}
