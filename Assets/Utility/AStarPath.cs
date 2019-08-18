using System.Collections.Generic;
using UnityEngine;

public interface INavigableTile
{
    bool IsUnlocked { get; }
    float MoveIntoCost { get; }
    Vector3 Position { get; }
    IEnumerable<INavigableTile> Neighbours { get; }
}

public class AStarPath
{
    public List<INavigableTile> ValidPath { get; private set; }
    public INavigableTile Start { get; private set; }
    public INavigableTile End { get; private set; }

    public AStarPath(List<INavigableTile> allTiles, INavigableTile startTile, INavigableTile endTile)
    {
        Start = startTile;
        End = endTile;
        ValidPath = FindPath(allTiles, startTile, endTile);
    }

    private float DefaultEstimate(INavigableTile a, INavigableTile b)
    {
        return Vector3.Distance(a.Position, b.Position);
    }
        
    private List<INavigableTile> FindPath(List<INavigableTile> allTiles, INavigableTile startTile, INavigableTile endTile)
    {
        if(allTiles.Contains(startTile) == false || allTiles.Contains(endTile) == false)
        {
            Debug.LogWarning("AStarPath: Attempting to find path but the start / end path are not part of the graph");
            return null;
        }

        //Variable init
        Dictionary<INavigableTile, INavigableTile> path = new Dictionary<INavigableTile, INavigableTile>();

        List<INavigableTile> closedSet = new List<INavigableTile>();

        PriorityQueue<float, INavigableTile> openSet = new PriorityQueue<float, INavigableTile>();
        openSet.Enqueue(0, startTile);

        //G Score Init
        Dictionary<INavigableTile, float> g_score = new Dictionary<INavigableTile, float>();

        foreach (INavigableTile h in allTiles)
        {
            g_score[h] = Mathf.Infinity;
        }

        g_score[startTile] = 0;

        //F score init
        Dictionary<INavigableTile, float> f_score = new Dictionary<INavigableTile, float>();

        foreach (INavigableTile h in allTiles)
        {
            f_score[h] = Mathf.Infinity;
        }

        f_score[startTile] = DefaultEstimate(startTile, endTile);

        //Search loop
        while (!openSet.IsEmpty)
        {
            INavigableTile current = openSet.Dequeue().Value;

            //path is complete
            if (current == endTile)
            {
                return ProcessPath(path, startTile, endTile);
            }

            closedSet.Add(current);

            foreach (var n in current.Neighbours)
            {

                //If move cost is infinite, or tile is impassable
                if (n == null || (n.IsUnlocked == false) || closedSet.Contains(n))
                {
                    continue;
                }

                float tentative_g_score = g_score[current] + n.MoveIntoCost;

                if (openSet.Contains(n))
                {
                    //a shorter path has been found before
                    if (tentative_g_score >= g_score[n])
                    {
                        continue;
                    }
                }

                path[n] = current;

                g_score[n] = tentative_g_score;
                f_score[n] = g_score[n] + DefaultEstimate(n, endTile);

                if (openSet.Contains(n) == false)
                {
                    openSet.Enqueue(f_score[n], n);
                }
            }
        }

        return null;
    }

    private List<INavigableTile> ProcessPath(Dictionary<INavigableTile, INavigableTile> tiles, INavigableTile start, INavigableTile end)
    {
        var path = new List<INavigableTile>();

        INavigableTile current = end;

        while(tiles.ContainsKey(current))
        {
            path.Add(current);
            current = tiles[current];
        }

        path.Reverse();

        return path;
    }
}
