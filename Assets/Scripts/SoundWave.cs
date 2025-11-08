using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool hitWall = false;
    private bool fadingOut = false;
    private float fadeSpeed = 1f; // Velocidade que a onda some
    public float expandSpeed = 2f; // Velocidade de expansão

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
{
    if (hitWall && !fadingOut)
    {
        fadingOut = true;
    }

    if (fadingOut)
    {
        Color color = spriteRenderer.color;
        color.a = Mathf.Max(0, color.a - fadeSpeed * Time.deltaTime);
        spriteRenderer.color = color;

        if (color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
    else
    {
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

        Color color = spriteRenderer.color;
        color.a = Mathf.Max(0, color.a - fadeSpeed * 0.2f * Time.deltaTime);
        spriteRenderer.color = color;
    }
}


    void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Wave hit: " + other.name + " | tag: " + other.tag);

    // Ignora player e outras ondas
    if (other.CompareTag("Player") || other.CompareTag("SoundWave")) return;

    if (other.CompareTag("Wall"))
    {
        Debug.Log("WALL DETECTED — fade-out iniciado");
        hitWall = true;
        expandSpeed = 0f;
    }
}
}
