using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXObject : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private AudioClip[] audioClips;
    AudioSource audioSource;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();    
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Start() 
    {
        Destroy(this.gameObject, lifeTime); 
        PlayeEffect();   
    }

    private void PlayeEffect()
    {
        ChooseClip();
        audioSource.Play();
    }

    private void ChooseClip()
    {
        int range = audioClips.Length;
        int clipNumber = Random.Range(0, range);

        if (clipNumber < audioClips.Length)
        {
            audioSource.clip = audioClips[clipNumber];
        }
    }
}
