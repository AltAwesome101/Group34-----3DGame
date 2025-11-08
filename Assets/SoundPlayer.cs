using UnityEngine;

public static class SoundPlayer2D
{
    public static void Play2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("Temp2DSound");
        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.volume = volume;
        source.Play();
        Object.Destroy(temp, clip.length);
    }
}
