using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MediumEnemyController : EnemyBase
{
    public Animator mediumEnemyAnimator;

    private float speed;
    public override void Start()
    {
        base.Start();
        speed = gameManager.basicEnemySpeed;
        enemyLives = 3;

        var block = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(block);
        block.SetFloat("_Switch", 1);
        enemyRenderer.SetPropertyBlock(block);

        SetColor(new Color(0.196f, 0.059f, 0.207f));
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
        Vector3 teleportPosition = player.transform.position - player.transform.forward * 3f;
        teleportPosition.y = transform.position.y;

        transform.position = teleportPosition;
        transform.LookAt(player.transform.position);

        getHit = true;
        Debug.Log("Medium enemy get hit");

        enemyLives--;
        if (enemyLives == 2)
        {
            gameManager.IncreaseScore(gameManager.scorePerHit);
            mediumEnemyAnimator.SetTrigger("GetHit");
            SetColor(yellowColor);
        }
        else if (enemyLives == 1)
        {
            SetColor(redColor);
        }
        else
        {
            gameManager.IncreaseScore(gameManager.scorePerEnemy);
            mediumEnemyAnimator.SetBool("IsDead", true);
            Die(0.0f);
        }
    }

}
