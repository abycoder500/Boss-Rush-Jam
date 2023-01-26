using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private float timeToWaitForGameStart = 1f;

    private Fader fader;
    private AudioManager audioManager;
    private Intro intro;

    private void Awake() 
    {
        fader = FindObjectOfType<Fader>();  
        audioManager = FindObjectOfType<AudioManager>();  
        intro = FindObjectOfType<Intro>();
    }

    private IEnumerator Start() 
    {
        yield return new WaitForSeconds(timeToWaitForGameStart);
        audioManager.PlayMenuMusic();
        intro.Begin(fader);
    }
}
