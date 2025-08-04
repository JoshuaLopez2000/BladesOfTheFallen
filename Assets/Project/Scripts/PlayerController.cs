using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Animator animator;
    public Material inkWaveMaterial;
    private float lastAttackTime = 0f;
    private List<int> attackIds = new List<int> { 0, 1, 2, 3 };
    private int attackId = 0;
    private void Awake()
    {
        instance = this;

    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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


    private void getAttackId()
    {
        if (Time.time - lastAttackTime > 2f)
        {
            attackIds = new List<int> { 0, 1, 2, 3 };
        }

        int newAttackId = attackIds[0];
        attackIds.RemoveAt(0);
        lastAttackTime = Time.time;
        Debug.Log("Attack ID: " + newAttackId);

        if (attackIds.Count == 0)
        {
            attackIds = new List<int> { 0, 1, 2, 3 };
        }
        attackId = newAttackId;
    }

    void PerformSlash()
    {
        getAttackId();
        animator.SetFloat("IdAttack", attackId);
        animator.SetTrigger("Attack");
    }

    public void EnemyHitted(GameObject enemy)
    {
        Debug.Log("Golpeaste a " + enemy.name);
        //Destroy(enemy);
    }
}
