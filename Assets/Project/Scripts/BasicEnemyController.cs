using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private GameObject player;
    public float speed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the player GameObject by its tag
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Translate the enemy to move towards the player
        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}
