using UnityEngine;

public class BasicEnemyController : EnemyBase
{
    [SerializeField] private Animator enemyAnimator;

    public override void Initialize(Transform playerTransform, float newSpeed, int newLives, Color initialColor)
    {
        SetVisualStyle(0f);
        base.Initialize(playerTransform, newSpeed, newLives, initialColor);
    }

    protected override void Start()
    {
        if (!isInitialized)
        {
            base.Start();
            if (speed <= 0f)
            {
                speed = GameManager.EnemySpeed;
            }
        }

        SetVisualStyle(0f);
    }

    private void Update()
    {
        UpdateEnemy(enemyAnimator);
    }

    public override void GiveSpace(int lives)
    {
        base.GiveSpace(lives);
        enemyAnimator.SetTrigger("GetHit");
        getHit = true;
    }

    public override void Hit()
    {
        if (getHit)
        {
            return;
        }

        getHit = true;
        if (enemyLives > 1)
        {
            enemyLives--;
            GameManager.IncreaseScore(GameManager.ScorePerHit);
            enemyAnimator.SetTrigger("GetHit");
            SetColor(redColor);
        }
        else
        {
            GameManager.IncreaseScore(GameManager.ScorePerEnemy);
            enemyAnimator.SetBool("IsDead", true);
            Die(0.1f);
        }
    }
}
