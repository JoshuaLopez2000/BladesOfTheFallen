using UnityEngine;

public class BasicEnemyController : EnemyBase
{
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // Translate the enemy to move towards the player
        if (player != null)
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
    }

    public override void Hit()
    {
        Debug.Log("Basic enemy hit");
        if (enemyLives > 1)
        {
            enemyLives--;
            gameManager.IncreaseScore(gameManager.scorePerHit);
            SetColor(Color.red);
        }
        else
        {
            gameManager.IncreaseScore(gameManager.scorePerEnemy);
            Die();
        }
    }
}
