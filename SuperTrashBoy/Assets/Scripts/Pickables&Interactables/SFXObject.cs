using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXObject : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private AudioClip audioClip;
    AudioSource audioSource;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();    
        audioSource.clip = audioClip;
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
        audioSource.Play();
    }
}
