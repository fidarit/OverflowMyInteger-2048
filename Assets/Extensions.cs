using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static Vector3 RoundToHalf(this Vector3 input, float y = 0)
    {
        var x = 0.5f * Mathf.Round(input.x * 2);
        var z = 0.5f * Mathf.Round(input.z * 2);

        return new Vector3(x, y, z);
    }

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> input)
    {
        return input.OrderBy(t => Random.Range(short.MinValue, short.MaxValue));
    }
}
