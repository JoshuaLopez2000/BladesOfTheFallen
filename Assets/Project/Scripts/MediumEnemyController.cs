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

            StartCoroutine(WaitAndTeleportBehindPlayer(0.1f));
        }
        else
        {
            GameManager.IncreaseScore(GameManager.ScorePerEnemy);
            mediumEnemyAnimator.SetBool("IsDead", true);
            Die(0f);
        }

        return true;
    }

    private IEnumerator WaitAndTeleportBehindPlayer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        float teleportDistance = Mathf.Max(
            0f,
            GameManager.PlayerAttackRange - GameManager.MediumEnemyTeleportRangeInset);
        Vector3 teleportPosition = player.position - player.forward * teleportDistance;
        teleportPosition.y = transform.position.y;
        transform.position = teleportPosition;
        FacePlayer();
    }
}
