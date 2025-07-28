using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject slashEffectPrefab;
    public Animator animator;
    public Material inkWaveMaterial;
    private bool inkShader = false;
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
            if (inkShader)
            {
                inkShader = false;
                inkWaveMaterial.SetFloat("_Switch", 1);
            }
            else
            {
                inkShader = true;
                inkWaveMaterial.SetFloat("_Switch", 0);

            }
            Debug.Log("Switch: " + inkWaveMaterial.GetFloat("_Switch"));

        }
    }

    void PerformSlash()
    {
        GameObject effect = Instantiate(slashEffectPrefab, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
    }
}
