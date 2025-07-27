using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject slashEffectPrefab;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Implement player input handling for slashing
        // For example, using the right arrow key to trigger a slash
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            animator.SetBool("HitEnemy", true);
            animator.SetInteger("IdAttack", 0);
            PerformSlash();
        }
    }

    void PerformSlash()
    {
        GameObject effect = Instantiate(slashEffectPrefab, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
    }
}
