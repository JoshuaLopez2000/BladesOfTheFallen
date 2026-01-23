using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BasicEnemyController : EnemyBase
{
    public Animator enemyAnimator;
    private float attackCooldown;
    private float lastAttackTime;

    public override void Initialize(Transform playerTransform, float newSpeed, int newLives, Color initialColor)
    {
        // Set visual style first (Switch 0 for Basic) to match original execution order
        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Switch", 0);
        enemyRenderer.SetPropertyBlock(propBlock);

        base.Initialize(playerTransform, newSpeed, newLives, initialColor);
    }

    public override void Start()
    {
        if (isInitialized) 
        {
            // Just initialize local cooldowns if skipped base.Start logic
            attackCooldown = 3.0f;
            lastAttackTime = 0.0f;
            return;
        }

        base.Start();
        if (speed == 0) speed = gameManager.enemySpeed;
        
        attackCooldown = 3.0f;
        lastAttackTime = 0.0f;

        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Switch", 0);
        enemyRenderer.SetPropertyBlock(propBlock);
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

    public override void GiveSpace(int lives)
    {
        base.GiveSpace(lives);
        enemyAnimator.SetTrigger("GetHit");
        getHit = true;
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
