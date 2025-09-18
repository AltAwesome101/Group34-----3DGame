using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasZone : MonoBehaviour
{
    [Header("Damage")]
    public int damagePerTick = 5;
    public float tickInterval = 1.0f;

    [Header("Effects")]
    public ParticleSystem gasParticles;
    public AudioSource leakAudio; 
    public float dissipateTime = 3f; 

    [Header("Coughing")]
    [Tooltip("Clips randomly played while the player is inside gas")]
    public AudioClip[] coughClips;
    public float coughInterval = 2.0f; 
    [Range(0f, 1f)] public float coughVolume = 1f;

    [Header("State")]
    public bool isLeaking = true;

    private HashSet<DamagePlayer> playersInZone = new HashSet<DamagePlayer>();

    private Coroutine damageCoroutine;

    private Coroutine coughCoroutine;

    private AudioSource coughSource;

    private void Awake()
    {
       
        if (gasParticles == null) gasParticles = GetComponentInChildren<ParticleSystem>();
        if (leakAudio == null) leakAudio = GetComponent<AudioSource>();

        coughSource = gameObject.AddComponent<AudioSource>();
        coughSource.playOnAwake = false;
        coughSource.spatialBlend = 1f;
        coughSource.volume = coughVolume;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLeaking) return;

        var dp = other.GetComponent<DamagePlayer>();
        if (dp != null)
        {
            playersInZone.Add(dp);

            if (damageCoroutine == null)
                damageCoroutine = StartCoroutine(DamageLoop());

            if (coughCoroutine == null && coughClips.Length > 0)
                coughCoroutine = StartCoroutine(CoughLoop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var dp = other.GetComponent<DamagePlayer>();
        if (dp != null)
        {
            playersInZone.Remove(dp);

            if (playersInZone.Count == 0)
            {
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }

                if (coughCoroutine != null)
                {
                    StopCoroutine(coughCoroutine);
                    coughCoroutine = null;
                }
            }
        }
    }

    private IEnumerator DamageLoop()
    {
        while (isLeaking)
        {
            var snapshot = new List<DamagePlayer>(playersInZone);
            foreach (var player in snapshot)
            {
                if (player != null)
                    player.ApplyDamage(damagePerTick);
            }
            yield return new WaitForSeconds(tickInterval);
        }
        damageCoroutine = null;
    }

    private IEnumerator CoughLoop()
    {
        while (isLeaking && playersInZone.Count > 0)
        {
            PlayRandomCough();
            yield return new WaitForSeconds(coughInterval);
        }
        coughCoroutine = null;
    }

    private void PlayRandomCough()
    {
        if (coughClips.Length == 0) return;
        var clip = coughClips[Random.Range(0, coughClips.Length)];
        if (clip != null) coughSource.PlayOneShot(clip);
    }

    public void CloseValve()
    {
        if (!isLeaking) return;
        isLeaking = false;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
        if (coughCoroutine != null)
        {
            StopCoroutine(coughCoroutine);
            coughCoroutine = null;
        }

        if (leakAudio != null) leakAudio.Stop();
        if (coughSource.isPlaying) coughSource.Stop();

        if (gasParticles != null)
            StartCoroutine(StopParticlesAfterDelay());
    }

    private IEnumerator StopParticlesAfterDelay()
    {
        var emission = gasParticles.emission;
        emission.enabled = false;
        yield return new WaitForSeconds(dissipateTime);
        gasParticles.Stop();
    }

    public void OpenValve()
    {
        if (isLeaking) return;
        isLeaking = true;

        if (gasParticles != null)
        {
            var emission = gasParticles.emission;
            emission.enabled = true;
            gasParticles.Play();
        }

        if (leakAudio != null) leakAudio.Play();

        if (playersInZone.Count > 0)
        {
            if (damageCoroutine == null)
                damageCoroutine = StartCoroutine(DamageLoop());
            if (coughClips.Length > 0 && coughCoroutine == null)
                coughCoroutine = StartCoroutine(CoughLoop());
        }
    }
}
