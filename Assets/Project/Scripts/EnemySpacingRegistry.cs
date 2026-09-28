using System.Collections.Generic;
using UnityEngine;

internal static class EnemySpacingRegistry
{
    private static readonly List<EnemyBase> ActiveEnemies = new();
    private static readonly Dictionary<EnemyBase, int> SortedIndices = new();
    private static int preparedFrame = -1;

    public static void Register(EnemyBase enemy)
    {
        if (enemy == null || ActiveEnemies.Contains(enemy))
        {
            return;
        }

        ActiveEnemies.Add(enemy);
        preparedFrame = -1;
    }

    public static void Unregister(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return;
        }

        ActiveEnemies.Remove(enemy);
        SortedIndices.Remove(enemy);
        preparedFrame = -1;
    }

    public static bool HasEnemyAhead(EnemyBase enemy, float movementDirectionX, float minimumDistance)
    {
        if (enemy == null || Mathf.Abs(movementDirectionX) <= Mathf.Epsilon || minimumDistance <= 0f)
        {
            return false;
        }

        PrepareForCurrentFrame();
        if (!SortedIndices.TryGetValue(enemy, out int enemyIndex))
        {
            return false;
        }

        int neighborIndex = movementDirectionX > 0f ? enemyIndex + 1 : enemyIndex - 1;
        if (neighborIndex < 0 || neighborIndex >= ActiveEnemies.Count)
        {
            return false;
        }

        float separation = Mathf.Abs(
            ActiveEnemies[neighborIndex].transform.position.x - enemy.transform.position.x);
        return separation < minimumDistance;
    }

    private static void PrepareForCurrentFrame()
    {
        if (preparedFrame == Time.frameCount)
        {
            return;
        }

        for (int index = ActiveEnemies.Count - 1; index >= 0; index--)
        {
            EnemyBase enemy = ActiveEnemies[index];
            if (enemy == null || !enemy.isActiveAndEnabled)
            {
                ActiveEnemies.RemoveAt(index);
            }
        }

        ActiveEnemies.Sort(ComparePositionX);
        SortedIndices.Clear();
        for (int index = 0; index < ActiveEnemies.Count; index++)
        {
            SortedIndices[ActiveEnemies[index]] = index;
        }

        preparedFrame = Time.frameCount;
    }

    private static int ComparePositionX(EnemyBase left, EnemyBase right)
    {
        return left.transform.position.x.CompareTo(right.transform.position.x);
    }
}
