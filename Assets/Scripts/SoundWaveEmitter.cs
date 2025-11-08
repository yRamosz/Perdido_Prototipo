using UnityEngine;

public class SoundWaveEmitter : MonoBehaviour
{
    public GameObject particlePrefab;
    public int particleCount = 100;
    public float spreadAngle = 360f;

    public void Emit()
    {
        for (int i = 0; i < particleCount; i++)
        {
            float angle = (spreadAngle / particleCount) * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(particlePrefab, transform.position, rotation);
        }
    }
}
