using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected GameObject player;

    public virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public abstract void Hit();

    protected void SetColor(Color color)
    {
        var renderer = GetComponent<Renderer>();
        var block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetColor("_Color", color);
        renderer.SetPropertyBlock(block);
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}
