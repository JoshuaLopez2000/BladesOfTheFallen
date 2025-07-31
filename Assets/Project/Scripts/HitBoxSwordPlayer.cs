using UnityEngine;

public class HitBoxSwordPlayer : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            playerController.EnemyHitted(other.gameObject);
        }
    }
}
