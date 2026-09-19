using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField, Min(0f)] private float floatSpeed = 1f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 1f;

    private TextMeshPro tmp;
    private Color originalColor;
    private float timer;

    public void Initialize(string text, Color color)
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.text = text;
        tmp.color = color;
        originalColor = color;
    }

    private void Update()
    {
        if (tmp == null)
        {
            return;
        }

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0f, timer / fadeDuration);
        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
