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
    protected bool getHit = false, resetting = false;

    protected Color redColor = new Color32(133, 28, 4, 255);
    protected Color yellowColor = new Color32(255, 198, 0, 255);


    private void OnEnable()
    {
        gameManager.OnPlayerLivesChanged += GiveSpace;
    }

    private void OnDisable()
    {
        gameManager.OnPlayerLivesChanged -= GiveSpace;
    }
    public virtual void Start()
    {
        attackRange = gameManager.basicEnemyAttackRange;
        maxHits = gameManager.maxHits;
        distanceBetweenEnemies = gameManager.distanceBetweenEnemies;

        player = GameObject.FindWithTag("Player");
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
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
        var block = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(block);
        block.SetColor("_Color", color);
        enemyRenderer.SetPropertyBlock(block);
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
