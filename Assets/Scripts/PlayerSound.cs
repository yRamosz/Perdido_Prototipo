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

    [Header("Pitch & Volume Randomization")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public float minVolume = 0.30f;
    public float maxVolume = 0.45f;

    [Header("Sound Wave Emitter")]
    public GameObject soundWaveEmitterPrefab; // arraste o prefab SoundWaveEmitter aqui

    private float stepTimer = 0f;

    void Update()
    {
        if (stepTimer > 0f) stepTimer -= Time.deltaTime;
    }

    public void PlayWalk()
    {
        TryPlayStep(walkSteps, walkInterval);
    }

    public void PlayRun()
    {
        TryPlayStep(runSteps, runInterval);
    }

    private void TryPlayStep(AudioClip[] clips, float interval)
    {
        if (clips == null || clips.Length == 0) return;

        if (stepTimer <= 0f)
        {
            // escolhe clip + pitch + volume
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.pitch = Random.Range(minPitch, maxPitch);
            float volume = Random.Range(minVolume, maxVolume);

            // toca o som
            source.PlayOneShot(clip, volume);

            // **EMITE A ONDA SINCRONIZADA**
            EmitWaveFromPlayer();

            // reseta timer
            stepTimer = interval;
        }
    }

    private void EmitWaveFromPlayer()
    {
        if (soundWaveEmitterPrefab == null)
        {
            Debug.LogWarning("[PlayerSound] soundWaveEmitterPrefab não atribuído!");
            return;
        }

        // instancia o emissor exatamente na posição do player e chama Emit()
        GameObject emitter = Instantiate(soundWaveEmitterPrefab, transform.position, Quaternion.identity);
        SoundWaveEmitter swe = emitter.GetComponent<SoundWaveEmitter>();
        if (swe != null)
        {
            swe.Emit();
        }
        else
        {
            Debug.LogWarning("[PlayerSound] prefab instanciado não contém SoundWaveEmitter.");
        }
    }
}
