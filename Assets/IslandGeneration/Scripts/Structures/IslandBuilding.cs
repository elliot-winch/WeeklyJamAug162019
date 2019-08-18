using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBuilding : IslandStructure
{
    public void Place(Vector2Int origin)
    {
        OriginPoint = origin;
        //Dimensions recorded in inspector

        Tiles = TryPlace(origin);
    }

    public List<Vector2Int> TryPlace(Vector2Int origin)
    {
        var coords = new List<Vector2Int>();

        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                coords.Add(new Vector2Int(origin.x + i, origin.y + j));
            }
        }

        return coords;
    }
}
