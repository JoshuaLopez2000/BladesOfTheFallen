using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected GameObject player;
    public GameManagerSO gameManager;

    protected float attackRange;
    protected float speed;
    protected int maxHits;
    protected float distanceBetweenEnemies;

    protected int enemyLives;

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
        var renderer = GetComponent<Renderer>();
        var block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetColor("_Color", color);
        renderer.SetPropertyBlock(block);
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}
