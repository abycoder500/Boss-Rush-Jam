using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip introMusic = null;
    [SerializeField] AudioClip dummyMusic = null;
    [SerializeField] AudioClip dummyWinMusic = null;
    [SerializeField] AudioClip jackMusic = null;
    [SerializeField] AudioClip kaijuMusic = null;
    [SerializeField] AudioClip outroMusic = null;

    [SerializeField] float stopMusicTime = 0.5f;

    AudioSource audioSource;

    Coroutine stopMusic;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(introMusic, true);
    }

    public void PlayOutroMusic()
    {
        PlayMusic(outroMusic, false);
    }

    public void StopMusic(Action AfterStop)
    {
        Debug.Log("music stop");
        stopMusic = StartCoroutine(ChangeVolume(stopMusicTime, 0f, AfterStop));
    }

    public void PlayDummyBossMusic()
    {
        PlayMusic(dummyMusic, true);
    }

    public void PlayJackBossMusic()
    {
        Debug.Log($"Audio: Jack Music started ({jackMusic.name})");
        PlayMusic(jackMusic, true);
    }

    public void PlayKaijuMusic()
    {
        PlayMusic(kaijuMusic, true);
    }

    public void PlayDummyWinMusic()
    {
        PlayMusic(dummyWinMusic, false);
    }

    private void PlayMusic(AudioClip audio, bool loop)
    {
        if (audio == null) return;
        if (null != stopMusic)
        {
            Debug.Log($"Nuking {stopMusic}");
            StopCoroutine(stopMusic);
        }
        audioSource.volume = 1f;
        audioSource.clip = audio;
        audioSource.loop = loop;
        audioSource.Play();
    }

    /*private IEnumerator ChangeVolume(float target, float time)
    {
        while(Mathf.Approximately(audioSource.volume, target))
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, target, Time.unscaledDeltaTime/time);
            yield return null;
        }
    }*/

    private IEnumerator ChangeVolume(float duration, float targetVolume, Action AfterStop)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (AfterStop != null) AfterStop();
        Debug.Log("ChangeVolume is done");
        yield break;
    }
}
