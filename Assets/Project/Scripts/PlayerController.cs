using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Animator animator;
    public Material inkWaveMaterial;
    public float attackRange = 2f, maxApproachDistance = 2.0f;
    private float lastAttackTime = 0f;
    private List<int> attackIdsAux = new List<int> { 0, 1, 2, 3 }, attackIds = new List<int>();
    private int attackId = 0;
    private void Awake()
    {
        instance = this;

    }
    void Start()
    {
        animator = GetComponent<Animator>();
        attackIds = new List<int>(attackIdsAux);
    }

    void Update()
    {
        // Debug raycast to check player position
        Debug.DrawRay(transform.position + Vector3.up * 0.2f, transform.forward * attackRange, Color.red);
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            PerformSlash();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            PerformSlash();
        }
    }

    public void inputRight()
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);
        PerformSlash();
    }

    public void inputLeft()
    {
        transform.rotation = Quaternion.Euler(0, 270, 0);
        PerformSlash();
    }


    private void getAttackId()
    {
        if (Time.time - lastAttackTime > 2f)
        {
            attackIds = new List<int>(attackIdsAux);
        }

        int newAttackId = attackIds[0];
        attackIds.RemoveAt(0);
        lastAttackTime = Time.time;
        Debug.Log("Attack ID: " + newAttackId);

        if (attackIds.Count == 0)
        {
            attackIds = new List<int>(attackIdsAux);
        }
        attackId = newAttackId;
    }

    void PerformSlash()
    {
        bool playerHitEnemy = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, transform.forward, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.Hit();
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
                playerHitEnemy = true;
            }
        }
        else
        {
            Debug.Log("No enemy hit, moving forward");
            float directionX = Mathf.Sign(transform.forward.x);
            Vector3 targetPosition = new Vector3(transform.position.x + directionX * attackRange, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackRange);
        }
        getAttackId();
        animator.SetFloat("idAttack", attackId);
        animator.SetTrigger("Attack");
        if (!playerHitEnemy)
        {
            // TODO: Play fail attack animation
        }
    }

    public void EnemyHitted(GameObject enemy)
    {
        Debug.Log("Golpeaste a " + enemy.name);
        //Destroy(enemy);
    }
}
