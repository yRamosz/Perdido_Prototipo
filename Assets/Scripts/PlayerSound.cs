using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;

    [Header("Footstep Variations - Walking")]
    public AudioClip[] walkSteps;
    public float walkInterval = 0.45f;

    [Header("Footstep Variations - Running")]
    public AudioClip[] runSteps;
    public float runInterval = 0.25f;

    [Header("Footstep Variations - Stealth")]
    public AudioClip[] stealthSteps;
    public float stealthInterval = 0.6f;

    [Header("Pitch & Volume Randomization")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public float minVolume = 0.30f;
    public float maxVolume = 0.45f;

    [Header("Emitters (prefabs)")]
    public GameObject normalWaveEmitterPrefab;
    public GameObject stealthWaveEmitterPrefab;

    private float stepTimer = 0f;

    void Update()
    {
        if (stepTimer > 0f) stepTimer -= Time.deltaTime;
    }

    public void PlayWalk()
    {
        TryPlayStep(walkSteps, walkInterval, false);
    }

    public void PlayRun()
    {
        TryPlayStep(runSteps, runInterval, false);
    }

    public void PlayStealth()
    {
        TryPlayStep(stealthSteps, stealthInterval, true);
    }

    public void ForceEmit(bool stealth)
    {
        EmitWave(stealth);
    }

    private void TryPlayStep(AudioClip[] clips, float interval, bool stealth)
    {
        if (stepTimer > 0f) return;

        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.pitch = Random.Range(minPitch, maxPitch);
            float volume = Random.Range(minVolume, maxVolume);
            source.PlayOneShot(clip, volume);
        }

        EmitWave(stealth);
        stepTimer = interval;
    }

    private void EmitWave(bool stealth)
    {
        GameObject prefab = stealth ? stealthWaveEmitterPrefab : normalWaveEmitterPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("[PlayerSound] Emissor não atribuído.");
            return;
        }

        GameObject emitter = Instantiate(prefab, transform.position, Quaternion.identity);

        SoundWaveEmitter swe = emitter.GetComponent<SoundWaveEmitter>();
        if (swe != null)
        {
            if (stealth)
            {
                swe.particleMultiplier = 0.4f;
                swe.scaleMultiplier = 0.7f;
                swe.baseParticleLifetime = 1f;
                swe.baseParticleSpeed = 2f;
            }
            else
            {
                swe.particleMultiplier = 1f;
                swe.scaleMultiplier = 1f;
                swe.baseParticleLifetime = 1.2f;
                swe.baseParticleSpeed = 6f;
            }

            swe.Emit();
        }
    }
}
