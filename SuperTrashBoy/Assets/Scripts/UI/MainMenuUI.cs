using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button quitGameButton;
    [SerializeField] Button creditsButton;

    private MySceneManager mySceneManager = null;

    private void Start()
    {
        mySceneManager = FindObjectOfType<MySceneManager>();
        OnEnable();
    }

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(() => mySceneManager.LoadFirstScene());
        quitGameButton.onClick.AddListener(() => mySceneManager.QuitGame());
        creditsButton.onClick.AddListener(() => mySceneManager.PlayCredits());
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveListener(() => mySceneManager.LoadFirstScene());
        quitGameButton.onClick.RemoveListener(() => mySceneManager.QuitGame());
        creditsButton.onClick.RemoveListener(() => mySceneManager.PlayCredits());
    }
}
