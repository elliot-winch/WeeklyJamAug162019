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

    public static T Random<T>(this List<T> v)
    {
        return v[(int)(UnityEngine.Random.value * v.Count)];
    }

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
}
