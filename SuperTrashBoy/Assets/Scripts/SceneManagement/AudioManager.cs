using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip introMusic = null;

    AudioSource audioSource;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void PlayMenuMusic()
    {
        if(introMusic == null) return;
        audioSource.clip = introMusic;
        audioSource.Play();
    }
}
