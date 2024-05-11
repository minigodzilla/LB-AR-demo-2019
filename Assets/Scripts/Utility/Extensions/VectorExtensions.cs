using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Returns a random value between the x and y values of the input vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float RandomRange(this Vector2 vector) => Random.Range(vector.x, vector.y);
}
