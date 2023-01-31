using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip introMusic = null;
    [SerializeField] AudioClip dummyMusic = null;

    AudioSource audioSource;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void PlayMenuMusic()
    {
        PlayMusic(introMusic, true);
    }

    public void PlayDummyBossMusic()
    {
        PlayMusic(dummyMusic, true);
    }

    private void PlayMusic(AudioClip audio, bool loop)
    {
        if (audio == null) return;
        audioSource.clip = audio;
        audioSource.loop = loop;
        audioSource.Play();
    }
}
