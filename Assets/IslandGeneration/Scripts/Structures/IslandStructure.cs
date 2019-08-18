using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandStructure : MonoBehaviour
{
    public Vector2Int OriginPoint { get; protected set; } //lower left corner
    public Vector2Int Dimensions => dimensions;

    [SerializeField] protected Vector2Int dimensions;

    public List<Vector2Int> Tiles { get; protected set; }

    public List<Vector2Int> AdjacentTiles()
    {
        List<Vector2Int> adjacentTiles = new List<Vector2Int>();

        for (int i = -1; i < dimensions.x + 1; i++)
        {
            adjacentTiles.Add(OriginPoint + new Vector2Int(i, -1));
            adjacentTiles.Add(OriginPoint + new Vector2Int(i, dimensions.y));
        }

        for (int i = -1; i < dimensions.y + 1; i++)
        {
            adjacentTiles.Add(OriginPoint + new Vector2Int(-1, i));
            adjacentTiles.Add(OriginPoint + new Vector2Int(dimensions.x, i));
        }

        return adjacentTiles;
    }
}
