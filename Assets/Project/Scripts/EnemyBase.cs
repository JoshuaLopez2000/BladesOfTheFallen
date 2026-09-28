using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected const float DefaultHitRecoveryDuration = 0.25f;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int StyleSwitchProperty = Shader.PropertyToID("_Switch");
    private static readonly int AttackState = Animator.StringToHash("Armature_Attack");

    [SerializeField] private GameManagerSO gameManager;
    protected Transform player;
    [SerializeField] protected Renderer enemyRenderer;
    [SerializeField, Min(0f)] private float parryStunDuration = 0.9f;
    [SerializeField, Min(0f)] private float parryPushDistance = 2.5f;

    [Header("Parry Telegraph")]
    [SerializeField] private Color parryCueColor = new Color32(80, 220, 255, 255);
    [SerializeField, Range(0f, 1f)] private float parryCueStartNormalized = 0.28f;
    [SerializeField, Range(0f, 1f)] private float parryCueEndNormalized = 0.58f;
    [SerializeField, Min(0.02f)] private float parryCueBlinkInterval = 0.08f;

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

    private GameObject attackHitbox;
    private Action<EnemyBase> releaseHandler;
    private Coroutine releaseRoutine;
    private float attackCooldown = 3f;
    private float lastAttackTime;
    private float pendingRecoveryDuration = DefaultHitRecoveryDuration;
    private float nextParryCueBlinkTime;
    private Color currentColor = Color.white;
    private bool parryCueActive;
    private bool showingParryCueColor;

    protected GameManagerSO GameManager => gameManager;
    protected abstract Animator EnemyAnimator { get; }

    protected virtual void Awake()
    {
        propBlock = new MaterialPropertyBlock();

        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("EnemyAttack"))
            {
                attackHitbox = child.gameObject;
                break;
            }
        }
    }

    private void OnEnable()
    {
        EnemySpacingRegistry.Register(this);

        if (gameManager != null)
        {
            gameManager.OnPlayerLivesChanged += GiveSpace;
        }
    }

    private void OnDisable()
    {
        EnemySpacingRegistry.Unregister(this);
        ClearParryCue();
        releaseRoutine = null;

        if (gameManager != null)
        {
            gameManager.OnPlayerLivesChanged -= GiveSpace;
        }
    }

    public virtual void Initialize(Transform playerTransform, float newSpeed, int newLives, Color initialColor)
    {
        StopAllCoroutines();
        EnemyAnimator.Rebind();
        EnemyAnimator.Update(0f);

        isInitialized = true;
        player = playerTransform;
        speed = newSpeed;
        enemyLives = newLives;
        getHit = false;
        resetting = false;
        pendingRecoveryDuration = DefaultHitRecoveryDuration;
        lastAttackTime = 0f;
        releaseRoutine = null;

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
        
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
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
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

        if (BeginHitRecovery(DefaultHitRecoveryDuration))
        {
            EnemyAnimator.SetTrigger("GetHit");
        }
    }

    public abstract bool TryHit();

    public bool TryParry()
    {
        if (!BeginHitRecovery(parryStunDuration))
        {
            return false;
        }

        EnemyAnimator.ResetTrigger("Attack");
        EnemyAnimator.SetTrigger("GetHit");

        Vector3 pushBack = -transform.forward * parryPushDistance;
        pushBack.y = 0f;
        transform.position += pushBack;
        gameManager.IncreaseScore(gameManager.ScorePerHit);
        return true;
    }

    protected void SetColor(Color color)
    {
        currentColor = color;
        ApplyColor(color);
    }

    private void ApplyColor(Color color)
    {
        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorProperty, color);
        enemyRenderer.SetPropertyBlock(propBlock);
    }

    protected void Die(float time)
    {
        gameManager.RegisterEnemyKilled();

        if (releaseRoutine != null)
        {
            return;
        }

        if (time <= 0f)
        {
            ReleaseOrDestroy();
            return;
        }

        releaseRoutine = StartCoroutine(ReleaseAfterDelay(time));
    }

    internal void SetReleaseHandler(Action<EnemyBase> handler)
    {
        releaseHandler = handler;
    }

    protected void SetVisualStyle(float style)
    {
        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(StyleSwitchProperty, style);
        enemyRenderer.SetPropertyBlock(propBlock);
    }

    protected void UpdateEnemy()
    {
        UpdateParryCue();

        if (player == null || getHit)
        {
            StartRecoveryIfNeeded();
            return;
        }

        Vector3 offsetToPlayer = player.position - transform.position;
        float sqrDistance = offsetToPlayer.sqrMagnitude;
        if (sqrDistance < attackRange * attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            EnemyAnimator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        Vector3 direction = offsetToPlayer.normalized;
        if (!EnemySpacingRegistry.HasEnemyAhead(this, direction.x, distanceBetweenEnemies))
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    protected void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = player.position - transform.position;
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
            StartCoroutine(WaitAndReset(pendingRecoveryDuration));
        }
    }

    protected bool BeginHitRecovery(float duration)
    {
        if (getHit)
        {
            return false;
        }

        getHit = true;
        pendingRecoveryDuration = Mathf.Max(0f, duration);
        ClearParryCue();
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        return true;
    }

    private void UpdateParryCue()
    {
        AnimatorStateInfo state = EnemyAnimator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = state.normalizedTime - Mathf.Floor(state.normalizedTime);
        bool cueShouldBeActive = state.shortNameHash == AttackState
            && normalizedTime >= parryCueStartNormalized
            && normalizedTime <= parryCueEndNormalized;

        if (!cueShouldBeActive)
        {
            ClearParryCue();
            return;
        }

        if (!parryCueActive)
        {
            parryCueActive = true;
            showingParryCueColor = false;
            nextParryCueBlinkTime = Time.time;
        }

        if (Time.time < nextParryCueBlinkTime)
        {
            return;
        }

        showingParryCueColor = !showingParryCueColor;
        ApplyColor(showingParryCueColor ? parryCueColor : currentColor);
        nextParryCueBlinkTime = Time.time + parryCueBlinkInterval;
    }

    private void ClearParryCue()
    {
        if (!parryCueActive)
        {
            return;
        }

        parryCueActive = false;
        showingParryCueColor = false;
        ApplyColor(currentColor);
    }

    protected IEnumerator WaitAndReset(float waitTime)
    {
        resetting = true;
        yield return new WaitForSeconds(waitTime);
        getHit = false;
        resetting = false;
    }

    private IEnumerator ReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        releaseRoutine = null;
        ReleaseOrDestroy();
    }

    private void ReleaseOrDestroy()
    {
        if (releaseHandler != null)
        {
            releaseHandler(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
