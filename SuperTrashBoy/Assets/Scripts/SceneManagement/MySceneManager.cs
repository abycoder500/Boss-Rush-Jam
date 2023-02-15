using System;
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
    private Outro outro;

    private CreditsRoll creditsRoll;
    private bool willPlayOutroSequence = false;

    private float cachedTimeScale;

    private void Awake()
    {
        fader = FindObjectOfType<Fader>();
        audioManager = FindObjectOfType<AudioManager>();
        HookupMenuScenePanels();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timeToWaitForGameStart);
        audioManager.PlayMenuMusic();
        intro.Begin(fader);
    }

    public void PauseGame()
    {
        cachedTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        Time.timeScale = cachedTimeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadScene(currentSceneIndex));
    }

    public void PlayCredits()
    {
        creditsRoll.gameObject.SetActive(enabled);
        creditsRoll.Begin(fader);
    }

    private void PlayOutro()
    {
        creditsRoll.gameObject.SetActive(enabled);
        outro.gameObject.SetActive(enabled);
        outro.Begin(fader); // This includes the credits roll!
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadMenuScene(bool withOutro = false)
    {
        Time.timeScale = cachedTimeScale;
        willPlayOutroSequence = withOutro;
        StartCoroutine(LoadScene(0));
    }

    public void LoadDummyScene()
    {
        Debug.Log("load the dummy scene");
        StartCoroutine(LoadScene(1));
    }

    public void LoadJackScene()
    {
        StartCoroutine(LoadScene(2));
    }

    public void LoadKaijuScene()
    {
        StartCoroutine(LoadScene(3));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        Cursor.lockState = CursorLockMode.Locked;

        yield return fader.FadeOut(loadSceneFadeOutTime);
        audioManager.StopMusic();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        // When we return to the main scene we need to refresh the references for the panels.
        // Also, if we return after the game is finished, this will run the outro sequence.
        if (0 == sceneIndex)
        {
            Cursor.lockState = CursorLockMode.Confined;
            HookupMenuScenePanels();
            if (willPlayOutroSequence)
            {
                PlayOutro();
            }
        }
        yield return new WaitForSeconds(loadSceneWaitTime);
        yield return fader.FadeIn(loadSceneFadeInTime);
    }


    private void HookupMenuScenePanels()
    {
        intro = FindObjectOfType<Intro>();
        outro = FindObjectOfType<Outro>();

        // Unlike intro/outro, creditsRoll disables itself, not its children
        creditsRoll = FindObjectOfType<CreditsRoll>(true);
    }
}
