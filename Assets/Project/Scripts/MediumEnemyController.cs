using System.Collections;
using UnityEngine;

public class MediumEnemyController : EnemyBase
{
    [SerializeField] private Animator mediumEnemyAnimator;
    protected override Animator EnemyAnimator => mediumEnemyAnimator;

    public override void Initialize(Transform playerTransform, float newSpeed, int newLives, Color initialColor)
    {
        SetVisualStyle(1f);
        base.Initialize(playerTransform, newSpeed, newLives, initialColor);
    }

    protected override void Start()
    {
        if (!isInitialized)
        {
            base.Start();
            speed = speed > 0f ? speed : GameManager.EnemySpeed;
            enemyLives = enemyLives > 0 ? enemyLives : 3;
        }

        SetVisualStyle(1f);
    }

    private void Update()
    {
        UpdateEnemy();
    }

    public override bool TryHit()
    {
        if (!BeginHitRecovery(DefaultHitRecoveryDuration))
        {
            return false;
        }

        enemyLives--;
        if (enemyLives > 0)
        {
            GameManager.IncreaseScore(GameManager.ScorePerHit);
            mediumEnemyAnimator.SetTrigger("GetHit");
            SetColor(enemyLives == 1 ? redColor : yellowColor);

            Vector3 teleportPosition = player.transform.position - player.transform.forward * GameManager.MediumEnemyTeleportDistance;
            teleportPosition.y = transform.position.y;
            StartCoroutine(WaitAndTeleport(0.1f, teleportPosition));
        }
        else
        {
            GameManager.IncreaseScore(GameManager.ScorePerEnemy);
            mediumEnemyAnimator.SetBool("IsDead", true);
            Die(0f);
        }

        return true;
    }

    private IEnumerator WaitAndTeleport(float waitTime, Vector3 position)
    {
        yield return new WaitForSeconds(waitTime);
        transform.position = position;
        FacePlayer();
    }
}
