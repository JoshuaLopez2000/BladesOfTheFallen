using UnityEngine;

public class ParallaxPrefab : MonoBehaviour
{
    public Transform target;
    [Range(0f, 1f)]
    public float parallaxFactor = 0.3f;
    public float smoothSpeed = 5f;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        if (target == null)
            target = Camera.main.transform;

        startPosition = transform.position;
        targetPosition = startPosition;
    }

    void Update()
    {
        float distanceX = (target.position.x - startPosition.x) * parallaxFactor;
        float distanceY = (target.position.y - startPosition.y) * parallaxFactor;

        targetPosition = new Vector3(startPosition.x + distanceX, startPosition.y + distanceY, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
