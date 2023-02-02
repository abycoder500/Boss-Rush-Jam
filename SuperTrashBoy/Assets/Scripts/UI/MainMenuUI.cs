using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;

    private MySceneManager mySceneManager = null;

    private void Start() 
    {
        mySceneManager = FindObjectOfType<MySceneManager>();
        startGameButton.onClick.AddListener(() => mySceneManager.LoadFirstScene());
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveListener(() => mySceneManager.LoadFirstScene());
    }
}
