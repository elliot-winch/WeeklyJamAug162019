using System.Collections.Generic;
using UnityEngine;

public class NavigablePoint : INavigableTile
{
    public bool IsUnlocked { get; set; }
    public float MoveIntoCost => Position.y;
    public Vector3 Position { get; set; }
    public IEnumerable<INavigableTile> Neighbours { get; set; }

    public Vector2Int GridPosition;
    public GameObject cube;

    public float DistanceFromCenter => Mathf.Sqrt(Position.x * Position.x + Position.z * Position.z);

    public void SetHeight(float y)
    {
        Position = new Vector3()
        {
            x = Position.x,
            z = Position.z,
            y = y
        };
    }

    public float DistanceFrom(NavigablePoint point)
    {
        return Vector3.Distance(Position, point.Position);
    }
}
