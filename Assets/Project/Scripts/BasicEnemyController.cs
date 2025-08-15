using System.Collections;
using UnityEngine;

public class BasicEnemyController : EnemyBase
{
    public Animator enemyAnimator;
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
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
            SetColor(Color.red);
        }
        else
        {
            gameManager.IncreaseScore(gameManager.scorePerEnemy);
            enemyAnimator.SetBool("IsDead", true);
            Die();
        }
    }
}
