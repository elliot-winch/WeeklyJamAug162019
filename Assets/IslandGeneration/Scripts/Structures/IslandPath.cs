using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandPath : IslandStructure
{
    public Direction Direction { get; private set; }

    public void Create(IslandTop surface, MinMax pathLength, Vector2Int crossingPoint, Direction dir)
    {
        //Determine a length for the path
        int length = (int)pathLength.GetRandomValue();

        //Determine how far the left / south extreme of the path will be
        int offset = (int)(Random.value * length);

        Vector2Int aCoord = Vector2Int.zero, bCoord = Vector2Int.zero;

        switch (dir)
        {
            case Direction.X:
                aCoord = crossingPoint + new Vector2Int(-offset, 0);
                bCoord = crossingPoint + new Vector2Int(length - offset, 0);

                dimensions = new Vector2Int(length, 1);
                break;
            case Direction.Z:
                aCoord = crossingPoint + new Vector2Int(0, -offset);
                bCoord = crossingPoint + new Vector2Int(0, length - offset);

                dimensions = new Vector2Int(1, length);
                break;
        }

        OriginPoint = aCoord;

        var pathGridCoords = GridUtility.DrawRasterLine(aCoord, bCoord);

        Direction = dir;

        Tiles = pathGridCoords.Select(x =>
        {
            if (surface.PointMap.ContainsKey(x))
            {
                return surface.PointMap[x].GridPosition;
            }
            return default;
        })
        .NonNull()
        .ToList();
    }
}
