using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button resumeButton;

    private MySceneManager mySceneManager = null;


    // Auto hide when the scene is started
    void Start()
    {
        mySceneManager = FindObjectOfType<MySceneManager>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        mainMenuButton.onClick.AddListener(() => mySceneManager.LoadMenuScene());
        resumeButton.onClick.AddListener(() => mySceneManager.ResumeGame());
    }

    private void OnDisable()
    {
        mainMenuButton.onClick.RemoveListener(() => mySceneManager.LoadMenuScene());
        resumeButton.onClick.RemoveListener(() => mySceneManager.ResumeGame());
    }

}
