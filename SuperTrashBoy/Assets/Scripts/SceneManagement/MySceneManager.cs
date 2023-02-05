using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    [SerializeField] private float timeToWaitForGameStart = 1f;
    [SerializeField] private float loadSceneFadeOutTime = 1f;
    [SerializeField] private float loadSceneWaitTime = 2f;
    [SerializeField] private float loadSceneFadeInTime = 1f;

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

    public void PauseGame()
    {
        //time.timescale = 0;
    }

    public void ResumeGame()
    {
        //time.timescale = 0;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadMenuScene()
    {

    }

    public void LoadFirstScene()
    {
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        yield return fader.FadeOut(loadSceneFadeOutTime);
        audioManager.StopMusic();
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        yield return new WaitForSeconds(loadSceneWaitTime);
        yield return fader.FadeIn(loadSceneFadeInTime);
    }
}
