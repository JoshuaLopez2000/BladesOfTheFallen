using System.Collections;
using System.Collections.Generic;
using BladesOfTheFallen.Core;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    private const float ComboTimeout = 1f;
    private const int ComboDisplayThreshold = 10;

    private static readonly int AttackIdParameter = Animator.StringToHash("idAttack");
    private static readonly int AttackState = Animator.StringToHash("Base Layer.Attack");
    private static readonly int ParryTrigger = Animator.StringToHash("Parry");
    private static readonly int GetHitTrigger = Animator.StringToHash("GetHit");
    private static readonly int HitEnemyParameter = Animator.StringToHash("HitEnemy");
    private static readonly int StyleSwitchProperty = Shader.PropertyToID("_Switch");

    [Header("Dependencies")]
    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Header("Combat Timing")]
    [SerializeField, Min(0f)] private float attackStartupDuration = 0.05f;
    [SerializeField, Min(0f)] private float attackActiveDuration = 0.08f;
    [SerializeField, Min(0f)] private float missedAttackRecoveryDuration = 0.55f;
    [SerializeField, Min(0f)] private float parryStartupDuration = 0.08f;
    [SerializeField, Min(0f)] private float parryActiveDuration = 0.28f;
    [SerializeField, Min(0f)] private float parryRecoveryDuration = 0.18f;
    [SerializeField, Min(0f)] private float parryCooldown = 1.25f;
    [SerializeField, Min(0f)] private float hitStunDuration = 0.3f;
    [SerializeField, Min(0f)] private float hitRecoveryDuration = 0.15f;
    [SerializeField, Min(0f)] private float damageInvulnerabilityDuration = 0.7f;

    [Header("Combat Feedback")]
    [SerializeField, Range(0f, 1f)] private float parrySlowFactor = 0.1f;
    [SerializeField, Min(0f)] private float parryTimeEffectDuration = 0.45f;

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
    private readonly PlayerCombatStateMachine combat = new();

    private MaterialPropertyBlock propertyBlock;
    private Coroutine combatRoutine;
    private Coroutine invulnerabilityRoutine;
    private float attackRange = 2f;
    private float maxApproachDistance = 2f;
    private float lastAttackTime;
    private float lastParryTime = float.NegativeInfinity;
    private int attackId;
    private int combo;
    private bool? appliedSpecialAbilityState;

    public PlayerCombatPhase CombatPhase => combat.Phase;
    public bool IsDamageInvulnerable => combat.IsDamageInvulnerable;

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
        combat.Reset();

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
        if (inputReader != null)
        {
            inputReader.OnSlashRight -= HandleSlashRight;
            inputReader.OnSlashLeft -= HandleSlashLeft;
            inputReader.OnParry -= HandleParry;
        }

        StopAllCoroutines();
        combatRoutine = null;
        invulnerabilityRoutine = null;
        combat.Reset();
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
        TryStartSlash(Quaternion.Euler(0f, 90f, 0f));
    }

    private void HandleSlashLeft()
    {
        TryStartSlash(Quaternion.Euler(0f, 270f, 0f));
    }

    private void TryStartSlash(Quaternion facing)
    {
        if (!combat.TryBeginAttack())
        {
            return;
        }

        transform.rotation = facing;
        animator.SetBool(HitEnemyParameter, false);
        SelectNextAttack();
        animator.SetFloat(AttackIdParameter, attackId);
        animator.Play(AttackState, 0, 0f);
        StartCombatRoutine(SlashSequence(transform.forward));
    }

    private void HandleParry()
    {
        if (Time.time - lastParryTime < parryCooldown || !combat.TryBeginParry())
        {
            return;
        }

        lastParryTime = Time.time;
        animator.SetBool(HitEnemyParameter, false);
        animator.SetTrigger(ParryTrigger);

        if (audioSource != null && parrySound != null)
        {
            audioSource.PlayOneShot(parrySound);
        }

        StartCombatRoutine(ParrySequence());
    }

    private IEnumerator SlashSequence(Vector3 direction)
    {
        yield return new WaitForSeconds(attackStartupDuration);
        if (!combat.TryOpenAttackWindow())
        {
            combatRoutine = null;
            yield break;
        }

        bool hitEnemy = TryHitEnemy(direction, attackRange, out EnemyBase enemy);
        if (hitEnemy)
        {
            ApproachEnemy(enemy.transform);
            CompleteSuccessfulAction(false);
        }
        else
        {
            MoveAfterMiss(direction);
            combo = 0;
            if (!combat.TryBeginMissRecovery())
            {
                combatRoutine = null;
                yield break;
            }

            yield return new WaitForSeconds(missedAttackRecoveryDuration);
            combat.TryBecomeReady();
            combatRoutine = null;
            yield break;
        }

        yield return new WaitForSeconds(attackActiveDuration);
        if (!combat.TryBeginRecovery())
        {
            combatRoutine = null;
            yield break;
        }

        combat.TryBecomeReady();
        combatRoutine = null;
    }

    private IEnumerator ParrySequence()
    {
        yield return new WaitForSeconds(parryStartupDuration);
        if (!combat.TryOpenParryWindow())
        {
            combatRoutine = null;
            yield break;
        }

        yield return new WaitForSeconds(parryActiveDuration);
        if (!combat.TryBeginRecovery())
        {
            combatRoutine = null;
            yield break;
        }

        yield return new WaitForSeconds(parryRecoveryDuration);
        combat.TryBecomeReady();
        combatRoutine = null;
    }

    private bool TryHitEnemy(Vector3 direction, float range, out EnemyBase enemy)
    {
        enemy = null;
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (!Physics.Raycast(
                origin,
                direction,
                out RaycastHit hit,
                range,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore)
            || !hit.collider.CompareTag("Enemy"))
        {
            return false;
        }

        enemy = hit.collider.GetComponentInParent<EnemyBase>();
        if (enemy == null || !enemy.TryHit())
        {
            enemy = null;
            return false;
        }

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

    private void MoveAfterMiss(Vector3 direction)
    {
        float directionX = Mathf.Sign(direction.x);
        Vector3 targetPosition = transform.position + Vector3.right * (directionX * attackRange);
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            Mathf.Max(0f, attackRange - maxApproachDistance));
    }

    private void CompleteSuccessfulAction(bool wasParry)
    {
        combo++;
        lastAttackTime = Time.time;

        if (wasParry)
        {
            gameManager.ParryTimeEffect(parrySlowFactor, parryTimeEffectDuration);
        }
        else
        {
            gameManager.HitTimeEffect(0.2f, 0.15f);
        }

        SpawnComboText();
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

        if (combat.Phase == PlayerCombatPhase.ParryActive)
        {
            EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
            if (enemy != null && enemy.TryParry())
            {
                animator.SetBool(HitEnemyParameter, true);
                CompleteSuccessfulAction(true);
            }

            return;
        }

        if (!combat.TryTakeDamage())
        {
            return;
        }

        InterruptCombatRoutine();
        animator.SetBool(HitEnemyParameter, false);
        animator.SetTrigger(GetHitTrigger);
        combo = 0;
        gameManager.DecreaseLife();
        StartCombatRoutine(HitReactionSequence());

        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
        }

        invulnerabilityRoutine = StartCoroutine(DamageInvulnerabilitySequence());
    }

    private IEnumerator HitReactionSequence()
    {
        yield return new WaitForSeconds(hitStunDuration);
        if (!combat.TryRecoverFromHit())
        {
            combatRoutine = null;
            yield break;
        }

        yield return new WaitForSeconds(hitRecoveryDuration);
        combat.TryBecomeReady();
        combatRoutine = null;
    }

    private IEnumerator DamageInvulnerabilitySequence()
    {
        yield return new WaitForSeconds(damageInvulnerabilityDuration);
        combat.EndDamageInvulnerability();
        invulnerabilityRoutine = null;
    }

    private void StartCombatRoutine(IEnumerator sequence)
    {
        InterruptCombatRoutine();
        combatRoutine = StartCoroutine(sequence);
    }

    private void InterruptCombatRoutine()
    {
        if (combatRoutine == null)
        {
            return;
        }

        StopCoroutine(combatRoutine);
        combatRoutine = null;
    }
}
