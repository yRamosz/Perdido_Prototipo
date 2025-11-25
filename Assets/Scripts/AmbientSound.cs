using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    public AudioSource source;
    public float fadeInTime = 3f;

    void Start()
    {
        source.volume = 0f;
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, 0.12f, t / fadeInTime);
            yield return null;
        }
    }
}
