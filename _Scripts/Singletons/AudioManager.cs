using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonBehaviour<AudioManager>
{
    [Header("Music Settings")]
    public AudioClip[] musicClips;
    public AudioMixerGroup musicMixer;
    public float musicVolume = 0.5f;

    [Header("Sound Effects Settings")]
    public AudioClip[] soundEffects;
    public AudioMixerGroup sfxMixer;
    public float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public override void Awake()
    {
        base.Awake();

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixer;
        musicSource.loop = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixer;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;

        StartCoroutine(PlayRandomMusic());
    }

    private IEnumerator PlayRandomMusic()
    {
        while (true)
        {
            if (!musicSource.isPlaying)
            {
                int randomIndex = Random.Range(0, musicClips.Length);
                musicSource.clip = musicClips[randomIndex];
                musicSource.Play();
            }

            yield return null;
        }
    }

    public void PlaySoundEffect(int index)
    {
        if (index < 0 || index >= soundEffects.Length)
        {
            Debug.LogError("Invalid sound effect index.");
            return;
        }
        if (!sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(soundEffects[index]);
        }
    }
}
