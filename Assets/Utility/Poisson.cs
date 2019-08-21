using System;
using System.Collections.Generic;
using UnityEngine;

public class Poisson
{
    public static List<Vector2Int> GeneratePoisson(int width, int height, float minDist, int numPoints, Func<Vector2Int, float> weightFunction = null)
    {
        var processList = new List<Vector2Int>();
        var samplePoints = new List<Vector2Int>();

        int randX = (int)(UnityEngine.Random.value * width - width / 2f);
        int randY = (int)(UnityEngine.Random.value * height - height / 2f);
        Vector2Int firstPoint = new Vector2Int(randX, randY);

        processList.Add(firstPoint);
        samplePoints.Add(firstPoint);

        //generate other points from points in queue.
        while (processList.Count > 0)
        {
            Vector2Int point = processList.RemoveRandom();

            for (int i = 0; i < numPoints; i++)
            {
                Vector2Int newPoint = weightFunction != null 
                    ? GenerateWeightedPoint(point, minDist, weightFunction) 
                    : GenerateRandomPointAround(point, minDist);

                //check that the point is in the image region
                //and no points exists in the point's neighbourhood
                if (InNeighbourhood(samplePoints, newPoint, minDist) == false)
                {
                    //update containers
                    processList.Add(newPoint);
                    samplePoints.Add(newPoint);
                }
            }
        }

        return samplePoints;
    }

    private static bool InNeighbourhood(List<Vector2Int> existingPoints, Vector2Int point, float minDist)
    {
        for (float i = -minDist / 2; i <= minDist / 2; i++)
        {
            for (float j = -minDist / 2; j <= minDist / 2; j++)
            {
                if (Mathf.Sqrt(i * i + j * j) > minDist)
                {
                    continue;
                }

                int x = (int)(point.x + i);
                int y = (int)(point.y + j);

                if (existingPoints.Contains(new Vector2Int(x, y)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static Vector2Int GenerateWeightedPoint(Vector2Int point, float minDist, Func<Vector2Int, float> weightFunc)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        //Search over 2 * minDist squares
        for (float i = -minDist; i <= minDist; i++)
        {
            for (float j = -minDist; j <= minDist; j++)
            {
                float dist = Mathf.Sqrt(i * i + j * j);

                if (dist > minDist && dist < 2 * minDist)
                {
                    points.Add(new Vector2Int((int)(point.x + i), (int)(point.y + j)));
                }
            }
        }

        return points.RandomWeighted(weightFunc);
    }

    private static Vector2Int GenerateRandomPointAround(Vector2Int point, float minDist)
    {
        //non-uniform, favours points closer to the inner ring, leads to denser packings
        double r1 = UnityEngine.Random.value; //random point between 0 and 1
        double r2 = UnityEngine.Random.value;
        //random radius between mindist and 2 * mindist
        double radius = minDist * (r1 + 1);
        //random angle
        double angle = 2 * Mathf.PI * r2;
        //the new point is generated around the point (x, y)
        double newX = point.x + radius * Math.Cos(angle);
        double newY = point.y + radius * Math.Sin(angle);
        return new Vector2Int((int)newX, (int)newY);
    }
}
