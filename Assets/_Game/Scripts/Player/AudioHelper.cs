using UnityEngine;

public static class AudioHelper
{
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.volume = volume;
        source.spatialBlend = 1f; // 3D звук (0 = 2D, 1 = 3D)
        source.PlayOneShot(clip);

        Object.Destroy(go, clip.length + 0.1f);
    }
}