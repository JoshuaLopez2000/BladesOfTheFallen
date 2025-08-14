using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Animator animator;
    public Material inkWaveMaterial;
    public float attackRange = 2f, maxApproachDistance = 2.0f;
    private float lastAttackTime = 0f;
    private List<int> attackIdsAux = new List<int> { 0, 1, 2 }, attackIds = new List<int>();
    private int attackId = 0;
    private bool playerCanHit = true, resetting = false;
    private static System.Random rng = new System.Random();

    private void Awake()
    {
        instance = this;

    }
    void Start()
    {
        animator = GetComponent<Animator>();
        attackIds = new List<int>(attackIdsAux);
        playerCanHit = true;
    }

    void Update()
    {
        // Debug raycast to check player position
        Debug.DrawRay(transform.position + Vector3.up * 0.2f, transform.forward * attackRange, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.2f, -transform.forward * attackRange, Color.blue);
        if (!playerCanHit && !resetting)
        {
            StartCoroutine(WaitAndReset(2f));
        }
    }

    public void inputRight()
    {
        if (playerCanHit)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            PerformSlash();
        }

    }

    public void inputLeft()
    {
        if (playerCanHit)
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            PerformSlash();
        }
    }

    public void inputUp()
    {
        if (playerCanHit)
        {
            PerformParry();
            playerCanHit = false;
        }
    }

    private void getAttackId()
    {
        if (Time.time - lastAttackTime > 2f)
        {
            attackIds = new List<int>(attackIdsAux);
            attackIds.Shuffle();
        }

        int newAttackId = attackIds[0];
        attackIds.RemoveAt(0);
        lastAttackTime = Time.time;
        Debug.Log("Attack ID: " + newAttackId);

        if (attackIds.Count == 0)
        {
            attackIds = new List<int>(attackIdsAux);
            attackIds.Shuffle();
        }
        attackId = newAttackId;
    }

    void PerformParry()
    {
        animator.SetTrigger("Parry");
        //TODO: Implement parry logic
    }
    void PerformSlash()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, transform.forward, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.Hit();
                    animator.SetBool("HitEnemy", true);
                }
                else
                {
                    Debug.Log("Raycast hit, but not an enemy");
                }

                Debug.Log("Enemy hit on raycast: " + hit.collider.name);
                float distanceX = Mathf.Abs(transform.position.x - hit.collider.transform.position.x);
                Debug.Log("Distance X: " + distanceX + ", Max Approach Distance: " + maxApproachDistance);

                if (distanceX > maxApproachDistance)
                {
                    Debug.Log("Approaching enemy");
                    // Calcula nueva posición sólo en X
                    Vector3 targetPosition = new Vector3(hit.collider.transform.position.x, transform.position.y, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, distanceX - maxApproachDistance);
                }
                playerCanHit = true;
            }
        }
        else
        {
            playerCanHit = false;
            Debug.Log("No enemy hit, moving forward");
            float directionX = Mathf.Sign(transform.forward.x);
            Vector3 targetPosition = new Vector3(transform.position.x + directionX * attackRange, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackRange - maxApproachDistance);
        }
        getAttackId();
        animator.SetFloat("idAttack", attackId);
        animator.SetTrigger("Attack");
        if (!playerCanHit)
        {
            animator.SetBool("HitEnemy", false);
        }
    }
    private IEnumerator WaitAndReset(float waitTime)
    {
        resetting = true;
        yield return new WaitForSeconds(waitTime);
        playerCanHit = true;
        resetting = false;
    }
}
