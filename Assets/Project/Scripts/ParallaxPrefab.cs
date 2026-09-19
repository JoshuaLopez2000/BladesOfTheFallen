using UnityEngine;

public class ParallaxPrefab : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(0f, 1f)] private float parallaxFactor = 0.3f;
    [SerializeField, Min(0f)] private float smoothSpeed = 5f;

    private Vector3 startPosition;

    private void Start()
    {
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }

        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float distanceX = (target.position.x - startPosition.x) * parallaxFactor;
        float distanceY = (target.position.y - startPosition.y) * parallaxFactor;
        Vector3 targetPosition = new(startPosition.x + distanceX, startPosition.y + distanceY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
