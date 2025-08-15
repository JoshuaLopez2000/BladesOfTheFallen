using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected GameObject player;
    public GameManagerSO gameManager;
    [SerializeField] protected Renderer enemyRenderer;

    protected float attackRange;
    protected float speed;
    protected int maxHits;
    protected float distanceBetweenEnemies;
    protected int enemyLives;
    protected bool getHit = false, resetting = false;

    public virtual void Start()
    {
        attackRange = gameManager.basicEnemyAttackRange;
        speed = gameManager.basicEnemySpeed;
        maxHits = gameManager.maxHits;
        distanceBetweenEnemies = gameManager.distanceBetweenEnemies;

        player = GameObject.FindWithTag("Player");
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        enemyLives = Random.Range(1, maxHits + 1);

        if (enemyLives > 1)
        {
            SetColor(Color.green);
        }
        else
        {
            SetColor(Color.red);
        }
    }

    public abstract void Hit();

    protected void SetColor(Color color)
    {
        var block = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(block);
        block.SetColor("_Color", color);
        enemyRenderer.SetPropertyBlock(block);
    }

    protected void Die()
    {
        gameManager.enemiesKilled++;
        Destroy(gameObject, gameManager.timeToDestroyEnemy);
    }

    protected IEnumerator WaitAndReset(float waitTime)
    {
        resetting = true;
        yield return new WaitForSeconds(waitTime);
        getHit = false;
        resetting = false;
    }
}
