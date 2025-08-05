using UnityEngine;

public class BasicEnemyController : EnemyBase
{
    public float speed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        // Find the player GameObject by its tag
        player = GameObject.FindWithTag("Player");
        var renderer = GetComponent<Renderer>();
        var block = new MaterialPropertyBlock();
        block.SetColor("_Color", Color.red);
        renderer.SetPropertyBlock(block);
    }

    void Update()
    {
        // Translate the enemy to move towards the player
        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public override void Hit()
    {
        Debug.Log("Basic enemy hit");
        SetColor(Color.yellow);
        Die();
    }
}
