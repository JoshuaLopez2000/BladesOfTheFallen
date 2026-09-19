using System.Collections.Generic;
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int index = list.Count - 1; index > 0; index--)
        {
            int randomIndex = UnityEngine.Random.Range(0, index + 1);
            (list[index], list[randomIndex]) = (list[randomIndex], list[index]);
        }
    }
}
