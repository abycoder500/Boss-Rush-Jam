using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausingManager : MonoBehaviour
{
    private PausePanelUI pausePanelUI;
    private InputManager inputManager;
    private MySceneManager mySceneManager;

    void Start()
    {
        pausePanelUI = FindObjectOfType<PausePanelUI>(true);
        mySceneManager = FindObjectOfType<MySceneManager>();
        inputManager = InputManager.Instance;

    }

    private void Update()
    {
        if (inputManager.IsPausing()) DoPause();
    }

    private void DoPause()
    {
        if (null != pausePanelUI) pausePanelUI.gameObject.SetActive(true);
        inputManager.enabled = false;
        mySceneManager.PauseGame();
    }

    public void OnResumeButton()
    {
        if (null != pausePanelUI) pausePanelUI.gameObject.SetActive(false);
        inputManager.enabled = true;
        mySceneManager.ResumeGame();
    }

    public void OnMainMenuButton()
    {
        mySceneManager.LoadMenuScene();
    }

    public void OnNextBossButton()
    {
        Debug.Log("Pause invokes next scene");
        OnResumeButton();
        mySceneManager.NextScene();
    }

}
