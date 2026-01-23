using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected GameObject player;
    public GameManagerSO gameManager;
    [SerializeField] protected Renderer enemyRenderer;

    protected float attackRange;
    protected int maxHits;
    protected float distanceBetweenEnemies;
    protected int enemyLives;
    protected float speed;
    protected bool getHit = false, resetting = false;

    protected Color redColor = new Color32(133, 28, 4, 255);
    protected Color yellowColor = new Color32(255, 198, 0, 255);

    protected MaterialPropertyBlock propBlock;
    protected bool isInitialized = false;

    protected virtual void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        gameManager.OnPlayerLivesChanged += GiveSpace;
    }

    private void OnDisable()
    {
        gameManager.OnPlayerLivesChanged -= GiveSpace;
    }

    public virtual void Initialize(Transform playerTransform, float newSpeed, int newLives, Color initialColor)
    {
        isInitialized = true;
        player = playerTransform.gameObject;
        speed = newSpeed;
        enemyLives = newLives;
        
        // Ensure stats are initialized
        attackRange = gameManager.basicEnemyAttackRange;
        maxHits = gameManager.maxHits;
        distanceBetweenEnemies = gameManager.distanceBetweenEnemies;
        
        // Ensure we look at the player immediately
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        SetColor(initialColor);
    }

    public virtual void Start()
    {
        if (isInitialized) return; // Skip default setup if already initialized

        attackRange = gameManager.basicEnemyAttackRange;
        maxHits = gameManager.maxHits;
        distanceBetweenEnemies = gameManager.distanceBetweenEnemies;

        // Fallback if Initialize wasn't called (e.g. placed in scene for testing)
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public virtual void GiveSpace(int lives)
    {
        float pushBackDistance = gameManager.distanceAfterHitPlayer;

        Vector3 pushBack = -transform.forward * pushBackDistance;

        pushBack.y = 0;

        transform.position += pushBack;

        Debug.Log(name + " pushed back, player lives remaining: " + lives);
    }

    public abstract void Hit();

    protected void SetColor(Color color)
    {
        enemyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color);
        enemyRenderer.SetPropertyBlock(propBlock);
    }

    protected void Die(float time)
    {
        gameManager.enemiesKilled++;
        Destroy(gameObject, time);
    }

    protected IEnumerator WaitAndReset(float waitTime)
    {
        resetting = true;
        yield return new WaitForSeconds(waitTime);
        getHit = false;
        resetting = false;
    }
}
