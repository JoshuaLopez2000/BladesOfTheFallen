using System.Collections.Generic;
using UnityEngine;

internal sealed class EnemyPool
{
    private readonly EnemyBase prefab;
    private readonly Transform container;
    private readonly Queue<EnemyBase> available = new();
    private readonly HashSet<EnemyBase> active = new();

    public EnemyPool(EnemyBase prefab, Transform container, int prewarmCount)
    {
        this.prefab = prefab;
        this.container = container;

        for (int index = 0; index < Mathf.Max(0, prewarmCount); index++)
        {
            available.Enqueue(CreateInstance());
        }
    }

    public EnemyBase Acquire(Vector3 position)
    {
        EnemyBase enemy = available.Count > 0 ? available.Dequeue() : CreateInstance();
        Transform enemyTransform = enemy.transform;
        enemyTransform.SetParent(container, true);
        enemyTransform.SetPositionAndRotation(position, Quaternion.identity);
        enemy.SetReleaseHandler(Release);
        active.Add(enemy);
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void Release(EnemyBase enemy)
    {
        if (enemy == null || !active.Remove(enemy))
        {
            return;
        }

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(container, true);
        available.Enqueue(enemy);
    }

    private EnemyBase CreateInstance()
    {
        EnemyBase enemy = Object.Instantiate(prefab, container);
        enemy.gameObject.SetActive(false);
        return enemy;
    }
}
