using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button resumeButton;

    private PausingManager pausingManager = null;

    // Auto hide when the scene is started
    void Start()
    {
        pausingManager = FindObjectOfType<PausingManager>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        mainMenuButton.onClick.AddListener(() => pausingManager.OnMainMenuButton());
        resumeButton.onClick.AddListener(() => pausingManager.OnResumeButton());
    }

    private void OnDisable()
    {
        mainMenuButton.onClick.RemoveListener(() => pausingManager.OnMainMenuButton());
        resumeButton.onClick.RemoveListener(() => pausingManager.OnResumeButton());
    }

}
