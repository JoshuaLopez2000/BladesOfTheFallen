using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BasicEnemyController : EnemyBase
{
    public Animator enemyAnimator;
    private float speed;
    private float attackCooldown;
    private float lastAttackTime;
    public override void Start()
    {
        base.Start();
        speed = gameManager.basicEnemySpeed;
        attackCooldown = 3.0f;
        lastAttackTime = 0.0f;
        var block = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(block);
        block.SetFloat("_Switch", 0);
        enemyRenderer.SetPropertyBlock(block);

        if (gameManager.enemiesKilled < 5)
        {
            gameManager.SetBasicEnemySpeed(1.0f);
            enemyLives = 1;
        }
        else if (gameManager.enemiesKilled < 10)
        {
            gameManager.SetBasicEnemySpeed(2.0f);
            if (Random.value < 0.3f)
            {
                enemyLives = 2;
            }
            else
            {
                enemyLives = 1;
            }
        }
        else
        {
            gameManager.SetBasicEnemySpeed(3.0f);
            if (Random.value < 0.65f)
            {
                enemyLives = 2;
            }
            else
            {
                enemyLives = 1;
            }
        }

        if (enemyLives > 1)
        {
            SetColor(yellowColor);
        }
        else
        {
            SetColor(redColor);
        }
    }

    void Update()
    {
        if ((Vector3.Distance(transform.position, player.transform.position) < attackRange) && !getHit && Time.time - lastAttackTime > attackCooldown)
        {
            enemyAnimator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
        if (player != null && !getHit)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), transform.forward, out hit, distanceBetweenEnemies))
        {
            if (hit.collider.CompareTag("Enemy") && hit.collider != this.GetComponent<Collider>())
            {
                transform.Translate(-Vector3.forward * speed * Time.deltaTime);
            }
        }

        if (getHit && !resetting)
        {
            StartCoroutine(WaitAndReset(0.5f));
        }
    }

    public override void Hit()
    {
        getHit = true;
        Debug.Log("Basic enemy get hit");
        if (enemyLives > 1)
        {
            enemyLives--;
            gameManager.IncreaseScore(gameManager.scorePerHit);
            enemyAnimator.SetTrigger("GetHit");
            SetColor(redColor);
        }
        else
        {
            gameManager.IncreaseScore(gameManager.scorePerEnemy);
            enemyAnimator.SetBool("IsDead", true);
            Die(0.1f);
        }
    }
}
