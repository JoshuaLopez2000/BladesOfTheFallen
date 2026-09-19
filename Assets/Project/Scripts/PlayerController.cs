using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    private const float AttackRecoveryDuration = 0.8f;
    private const float ComboTimeout = 1f;
    private const float ParryCooldown = 3f;
    private const int ComboDisplayThreshold = 10;

    private static readonly int AttackIdParameter = Animator.StringToHash("idAttack");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int ParryTrigger = Animator.StringToHash("Parry");
    private static readonly int GetHitTrigger = Animator.StringToHash("GetHit");
    private static readonly int HitEnemyParameter = Animator.StringToHash("HitEnemy");
    private static readonly int StyleSwitchProperty = Shader.PropertyToID("_Switch");

    [Header("Dependencies")]
    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Header("Audio")]
    [FormerlySerializedAs("slashs")]
    [SerializeField] private List<AudioClip> slashClips = new();
    [SerializeField] private AudioClip parrySound;

    [Header("Visuals")]
    [SerializeField] private Renderer playerInkWaveRenderer;
    [SerializeField] private Renderer katanaInkWaveRenderer;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private bool showDebugRays;

    private readonly List<int> attackIds = new() { 0, 1, 2 };
    private MaterialPropertyBlock propertyBlock;

    private float attackRange = 2f;
    private float maxApproachDistance = 2f;
    private float lastAttackTime;
    private float lastParryTime = -ParryCooldown;
    private int attackId;
    private int combo;
    private bool playerCanHit = true;
    private bool resetting;
    private bool? appliedSpecialAbilityState;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        if (inputReader == null)
        {
            return;
        }

        inputReader.OnSlashRight += HandleSlashRight;
        inputReader.OnSlashLeft += HandleSlashLeft;
        inputReader.OnParry += HandleParry;
    }

    private void OnDisable()
    {
        if (inputReader == null)
        {
            return;
        }

        inputReader.OnSlashRight -= HandleSlashRight;
        inputReader.OnSlashLeft -= HandleSlashLeft;
        inputReader.OnParry -= HandleParry;
    }

    private void Start()
    {
        attackIds.Shuffle();
        attackRange = gameManager.PlayerAttackRange;
        maxApproachDistance = gameManager.PlayerMaxApproachDistance;
        ApplySpecialAbilityVisual(gameManager.HasSpecialAbility);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (showDebugRays)
        {
            Vector3 origin = transform.position + Vector3.up * 0.2f;
            Debug.DrawRay(origin, transform.forward * attackRange, Color.red);
            Debug.DrawRay(origin, -transform.forward * attackRange, Color.blue);
        }
#endif

        if (!playerCanHit && !resetting)
        {
            StartCoroutine(WaitAndReset());
        }

        ApplySpecialAbilityVisual(gameManager.HasSpecialAbility);

        if (combo > 0 && Time.time - lastAttackTime > ComboTimeout)
        {
            combo = 0;
        }
    }

    public void InputRight()
    {
        if (inputReader != null)
        {
            inputReader.RaiseSlashRight();
        }
        else
        {
            HandleSlashRight();
        }
    }

    public void InputLeft()
    {
        if (inputReader != null)
        {
            inputReader.RaiseSlashLeft();
        }
        else
        {
            HandleSlashLeft();
        }
    }

    public void InputUp()
    {
        if (inputReader != null)
        {
            inputReader.RaiseParry();
        }
        else
        {
            HandleParry();
        }
    }

    private void HandleSlashRight()
    {
        if (!playerCanHit)
        {
            return;
        }

        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        PerformSlash();
    }

    private void HandleSlashLeft()
    {
        if (!playerCanHit)
        {
            return;
        }

        transform.rotation = Quaternion.Euler(0f, 270f, 0f);
        PerformSlash();
    }

    private void HandleParry()
    {
        if (playerCanHit)
        {
            PerformParry();
        }
    }

    private void PerformSlash()
    {
        bool hitEnemy = TryHitEnemy(transform.forward, attackRange, out RaycastHit hit);
        if (hitEnemy)
        {
            ApproachEnemy(hit.collider.transform);
        }
        else
        {
            playerCanHit = false;
            float directionX = Mathf.Sign(transform.forward.x);
            Vector3 targetPosition = transform.position + Vector3.right * (directionX * attackRange);
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                Mathf.Max(0f, attackRange - maxApproachDistance));
        }

        SelectNextAttack();
        animator.SetFloat(AttackIdParameter, attackId);
        animator.SetTrigger(AttackTrigger);
        CompleteAttack(hitEnemy);
    }

    private void PerformParry()
    {
        if (Time.time - lastParryTime < ParryCooldown)
        {
            return;
        }

        lastParryTime = Time.time;
        float parryRange = attackRange * 0.5f;
        bool hitEnemy = TryHitEnemy(transform.forward, parryRange, out _);
        hitEnemy |= TryHitEnemy(-transform.forward, parryRange, out _);

        animator.SetTrigger(ParryTrigger);
        if (audioSource != null && parrySound != null)
        {
            audioSource.PlayOneShot(parrySound);
        }

        CompleteAttack(hitEnemy);
    }

    private bool TryHitEnemy(Vector3 direction, float range, out RaycastHit hit)
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (!Physics.Raycast(origin, direction, out hit, range) || !hit.collider.CompareTag("Enemy"))
        {
            return false;
        }

        if (!hit.collider.TryGetComponent(out EnemyBase enemy))
        {
            return false;
        }

        enemy.Hit();
        animator.SetBool(HitEnemyParameter, true);
        return true;
    }

    private void ApproachEnemy(Transform enemyTransform)
    {
        float distanceX = Mathf.Abs(transform.position.x - enemyTransform.position.x);
        if (distanceX <= maxApproachDistance)
        {
            return;
        }

        Vector3 targetPosition = new(enemyTransform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            distanceX - maxApproachDistance);
    }

    private void CompleteAttack(bool hitEnemy)
    {
        if (hitEnemy)
        {
            playerCanHit = true;
            combo++;
            gameManager.HitTimeEffect(0.2f, 0.15f);
            SpawnComboText();
            return;
        }

        combo = 0;
        animator.SetBool(HitEnemyParameter, false);
    }

    private void SelectNextAttack()
    {
        if (Time.time - lastAttackTime > 2f)
        {
            ResetAttackIds();
        }

        attackId = attackIds[0];
        attackIds.RemoveAt(0);
        lastAttackTime = Time.time;

        if (attackIds.Count == 0)
        {
            ResetAttackIds();
        }

        if (audioSource != null && slashClips.Count > 0)
        {
            AudioClip clip = slashClips[Random.Range(0, slashClips.Count)];
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

    private void ResetAttackIds()
    {
        attackIds.Clear();
        attackIds.Add(0);
        attackIds.Add(1);
        attackIds.Add(2);
        attackIds.Shuffle();
    }

    private void SpawnComboText()
    {
        if (combo < ComboDisplayThreshold || floatingTextPrefab == null)
        {
            return;
        }

        GameObject textObject = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 6f, Quaternion.identity);
        if (textObject.TryGetComponent(out FloatingText floatingText))
        {
            floatingText.Initialize($"Combo x{combo}", Color.white);
        }
    }

    private void ApplySpecialAbilityVisual(bool enabled)
    {
        if (appliedSpecialAbilityState == enabled)
        {
            return;
        }

        ApplyStyleSwitch(playerInkWaveRenderer, enabled ? 1f : 0f);
        ApplyStyleSwitch(katanaInkWaveRenderer, enabled ? 1f : 0f);
        appliedSpecialAbilityState = enabled;
    }

    private void ApplyStyleSwitch(Renderer targetRenderer, float value)
    {
        if (targetRenderer == null)
        {
            return;
        }

        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(StyleSwitchProperty, value);
        targetRenderer.SetPropertyBlock(propertyBlock);
        propertyBlock.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyAttack"))
        {
            return;
        }

        animator.SetTrigger(GetHitTrigger);
        playerCanHit = false;
        combo = 0;
        gameManager.DecreaseLife();
    }

    private IEnumerator WaitAndReset()
    {
        resetting = true;
        yield return new WaitForSeconds(AttackRecoveryDuration);
        playerCanHit = true;
        resetting = false;
    }
}
