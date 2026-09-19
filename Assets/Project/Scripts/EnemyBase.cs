using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    private const float HitRecoveryDuration = 0.5f;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int StyleSwitchProperty = Shader.PropertyToID("_Switch");

    [SerializeField] private GameManagerSO gameManager;
    protected GameObject player;
    [SerializeField] protected Renderer enemyRenderer;

    protected float attackRange;
    protected float distanceBetweenEnemies;
    protected int enemyLives;
    protected float speed;
    protected bool getHit;
    protected bool resetting;

    protected Color redColor = new Color32(133, 28, 4, 255);
    protected Color yellowColor = new Color32(255, 198, 0, 255);

    protected MaterialPropertyBlock propBlock;
    protected bool isInitialized;

    private Collider enemyCollider;
    private float attackCooldown = 3f;
    private float lastAttackTime;

    protected GameManagerSO GameManager => gameManager;

    protected virtual void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        enemyCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerLivesChanged += GiveSpace;
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerLivesChanged -= GiveSpace;
        }
    }

    public virtual void Initialize(Transform playerTransform, float newSpeed, int newLives, Color initialColor)
    {
        isInitialized = true;
        player = playerTransform.gameObject;
        speed = newSpeed;
        enemyLives = newLives;
        
        attackRange = gameManager.BasicEnemyAttackRange;
        distanceBetweenEnemies = gameManager.DistanceBetweenEnemies;

        FacePlayer();

        SetColor(initialColor);
    }

    protected virtual void Start()
    {
        if (isInitialized)
        {
            return;
        }

        attackRange = gameManager.BasicEnemyAttackRange;
        distanceBetweenEnemies = gameManager.DistanceBetweenEnemies;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                FacePlayer();
            }
        }
    }

    public virtual void GiveSpace(int lives)
    {
        float pushBackDistance = gameManager.DistanceAfterPlayerHit;

        Vector3 pushBack = -transform.forward * pushBackDistance;

        pushBack.y = 0;

        transform.position += pushBack;
    }

    public abstract void Hit();

    protected void SetColor(Color color)
    {
        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorProperty, color);
        enemyRenderer.SetPropertyBlock(propBlock);
    }

    protected void Die(float time)
    {
        gameManager.RegisterEnemyKilled();
        Destroy(gameObject, time);
    }

    protected void SetVisualStyle(float style)
    {
        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(StyleSwitchProperty, style);
        enemyRenderer.SetPropertyBlock(propBlock);
    }

    protected void UpdateEnemy(Animator enemyAnimator)
    {
        if (player == null || getHit)
        {
            StartRecoveryIfNeeded();
            return;
        }

        float sqrDistance = (player.transform.position - transform.position).sqrMagnitude;
        if (sqrDistance < attackRange * attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            enemyAnimator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, transform.forward, out RaycastHit hit, distanceBetweenEnemies)
            && hit.collider.CompareTag("Enemy")
            && hit.collider != enemyCollider)
        {
            transform.Translate(-transform.forward * speed * Time.deltaTime, Space.World);
        }
    }

    protected void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }

    private void StartRecoveryIfNeeded()
    {
        if (getHit && !resetting)
        {
            StartCoroutine(WaitAndReset(HitRecoveryDuration));
        }
    }

    protected IEnumerator WaitAndReset(float waitTime)
    {
        resetting = true;
        yield return new WaitForSeconds(waitTime);
        getHit = false;
        resetting = false;
    }
}
