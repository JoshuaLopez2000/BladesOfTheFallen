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
        base.Initialize(playerTransform, newSpeed, newLives, initialColor);
        // Ensure the shader switch is set correctly for this enemy type
        var block = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(block);
        block.SetFloat("_Switch", 0);
        enemyRenderer.SetPropertyBlock(block);
    }

    public override void Start()
    {
        base.Start();
        // Default values if not initialized via Spawner (e.g. placed in scene)
        if (speed == 0) speed = gameManager.enemySpeed;
        
        attackCooldown = 3.0f;
        lastAttackTime = 0.0f;
        
        // If Initialize wasn't called, we might need to set the switch here too, 
        // but it's safe to set it in both or just check.
        // For simplicity, we ensure the visual state is correct here as fallback.
        var block = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(block);
        block.SetFloat("_Switch", 0);
        enemyRenderer.SetPropertyBlock(block);
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
