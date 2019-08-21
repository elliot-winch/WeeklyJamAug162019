using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T> v)
    {
        return v.Where(x => x != null);
    }

    #region Random
    public static List<T> Random<T>(this List<T> v, int count)
    {
        var operationsList = v.ToList();
        var selected = new List<T>();

        if (count > v.Count)
        {
            return operationsList;
        }

        for (int i = 0; i < count; i++)
        {
            var index = operationsList.RandomIndex();

            selected.Add(operationsList[index]);

            operationsList.RemoveAt(index);
        }

        return selected;
    }

    public static T Random<T>(this List<T> v)
    {
        return v.Count != 0 ? v[RandomIndex(v)] : default;
    }

    public static int RandomIndex<T>(this List<T> v)
    {
        return (int)(UnityEngine.Random.value * v.Count);
    }

    public static T RemoveRandom<T>(this List<T> v)
    {
        int r = v.RandomIndex();
        T o = v[r];
        v.RemoveAt(r);
        return o;
    }

    public static T RandomWeighted<T>(this List<T> v, Func<T, float> weightFunction)
    {
        if(v.Count <= 0)
        {
            return default;
        }

        float totalWeight = v.Sum(x => weightFunction(x));

        if(totalWeight <= 0f)
        {
            Debug.Log("RandomWeight: Requires a positive weight");
            return default;
        }

        float rand = UnityEngine.Random.value * totalWeight;
        float count = 0f;
        int index = -1;

        while(count < rand)
        {
            index++;
            count += weightFunction(v[index]);
        }

        return v[index];
    }
    #endregion

    #region Next
    public static T Next<T>(this List<T> v, T current)
    {
        return v[NextIndex(v.IndexOf(current), v.Count)];
    }

    public static T NextWhere<T>(this List<T> v, T current, Predicate<T> condition)
    {
        int i = NextIndex(v.IndexOf(current), v.Count);
        int examined = 0;

        while(condition(v[i]) == false)
        {
            i = NextIndex(i, v.Count);
            examined++;

            if(examined >= v.Count)
            {
                return default;
            }
        }

        return v[i];
    }

    private static int NextIndex(int current, int count)
    {
        return (current + 1) % count;
    }

    public static T Last<T>(this List<T> v)
    {
        return v.Count > 0 ? v[v.Count - 1] : default;
    }
    #endregion

    #region Unity
    public static void DestroyAll(this List<GameObject> v)
    {
        foreach(var g in v)
        {
            UnityEngine.Object.Destroy(g);
        }

        v.Clear();
    }

    public static void DestroyAll<T>(this List<T> v) where T : Component
    {
        var objs = v.OfType<Component>();
        
        foreach(var obj in objs)
        {
            UnityEngine.Object.Destroy(obj.gameObject);
        }
        
        v.Clear();
    }
    #endregion
}
