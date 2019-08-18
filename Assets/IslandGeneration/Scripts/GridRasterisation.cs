using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtility
{
    public static List<Vector2Int> DrawRasterLine(Vector2Int a, Vector2Int b)
    { 
        List<Vector2Int> line = new List<Vector2Int>();

        float distance = Mathf.Ceil(Vector2.Distance(a, b)) + 1;

        for(int i = 0; i < distance; i++)
        {
            Vector2 coord = Vector2.Lerp(a, b, i / distance);
            Vector2Int coordInt = new Vector2Int((int)coord.x, (int)coord.y);

            if (line.Contains(coordInt) == false)
            {
                line.Add(coordInt);
            }
        }

        return line;
    }
}
