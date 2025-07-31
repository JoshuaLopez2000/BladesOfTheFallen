using UnityEngine;

public class HitBoxSwordPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            PlayerController.instance.EnemyHitted(other.gameObject);
        }
    }
}
