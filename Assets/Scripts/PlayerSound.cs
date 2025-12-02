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
    public AudioClip[] stealthSteps;       // passos abafados (pode usar um arquivo curto)
    public float stealthInterval = 0.6f;   // passos mais espaçados

    [Header("Pitch & Volume Randomization")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public float minVolume = 0.30f;
    public float maxVolume = 0.45f;

    [Header("Emitters (prefabs)")]
    public GameObject normalWaveEmitterPrefab;   // prefab para onda normal
    public GameObject stealthWaveEmitterPrefab;  // prefab para onda stealth (menos partículas)

    private float stepTimer = 0f;

    void Update()
    {
        if (stepTimer > 0f) stepTimer -= Time.deltaTime;
    }

    // chamadas externas
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

    // Força emissão imediata (ex.: tecla espaço)
    public void ForceEmit(bool stealth)
    {
        // não toca áudio, só emite a onda visual (chamada quando apertar espaço)
        EmitWave(stealth);
    }

    private void TryPlayStep(AudioClip[] clips, float interval, bool stealth)
    {
        if (clips == null || clips.Length == 0) 
        {
            // mesmo sem áudio, queremos emitir a onda (se desejado)
            EmitWave(stealth);
            stepTimer = interval;
            return;
        }

        if (stepTimer <= 0f)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.pitch = Random.Range(minPitch, maxPitch);
            float volume = Random.Range(minVolume, maxVolume);

            source.PlayOneShot(clip, volume);

            // emitir onda sincronizada com o passo
            EmitWave(stealth);

            stepTimer = interval;
        }
    }

    private void EmitWave(bool stealth)
    {
        GameObject prefab = stealth ? stealthWaveEmitterPrefab : normalWaveEmitterPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("[PlayerSound] Emissor não atribuído para " + (stealth ? "stealth" : "normal"));
            return;
        }

        GameObject emitter = Instantiate(prefab, transform.position, Quaternion.identity);

        // aqui você pode ajustar multiplicadores dinamicamente (ex.: stealth menor)
        SoundWaveEmitter swe = emitter.GetComponent<SoundWaveEmitter>();
        if (swe != null)
        {
            if (stealth)
            {
                // ajusta para ondas menores e menos partículas
                swe.particleMultiplier = 0.3f;
                swe.scaleMultiplier = 0.5f;
                swe.baseParticleLifetime = 0.9f;
                swe.baseParticleSpeed = 4f;
            }
            else
            {
                // valores de default já no prefab; você pode ajustar dinamicamente se quiser
                swe.particleMultiplier = 1f;
                swe.scaleMultiplier = 1f;
                swe.baseParticleLifetime = 1.2f;
                swe.baseParticleSpeed = 6f;
            }

            // finalmente emite
            swe.Emit();
        }
        else
        {
            Debug.LogWarning("[PlayerSound] prefab instanciado não contém SoundWaveEmitter.");
        }
    }
}
