using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button quitGameButton;

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
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveListener(() => mySceneManager.LoadFirstScene());
        quitGameButton.onClick.RemoveListener(() => mySceneManager.QuitGame());
    }
}
