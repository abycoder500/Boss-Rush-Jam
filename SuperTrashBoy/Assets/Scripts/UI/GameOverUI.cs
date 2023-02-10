using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float timeToShowPanel = 1f;
    [SerializeField] float delayPanel = 0.3f;
    [SerializeField] float persitanceTimeOnScreen = 3f;
    [SerializeField] float timeToShowText = 2f;
    [SerializeField] float disappearanceTime = 0.5f;
    [SerializeField] float targetPanelAlpha = 0.9f;

    private MySceneManager mySceneManager;

    private void Awake() 
    {
        mySceneManager = FindObjectOfType<MySceneManager>();    
    }

    private void Start() 
    {
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        text.alpha = 0;
    }

    public void YouDied()
    {
        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        StartCoroutine(ChangePanelAlpha(targetPanelAlpha, timeToShowPanel));
        yield return new WaitForSeconds(delayPanel);
        StartCoroutine(ChangeTextAlpha(1f, timeToShowText));
        yield return new WaitForSeconds(persitanceTimeOnScreen);
        StartCoroutine(ChangePanelAlpha(0f, disappearanceTime));
        StartCoroutine(ChangeTextAlpha(0f, disappearanceTime));
        mySceneManager.ReloadCurrentScene();
    }

    private IEnumerator ChangePanelAlpha(float target, float time)
    {
        while (!Mathf.Approximately(panel.color.a, target))
        {
            float a = Mathf.MoveTowards(panel.color.a, target, Time.unscaledDeltaTime / time);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, a);
            yield return null;
        }
    }

    private IEnumerator ChangeTextAlpha(float target, float time)
    {
        while (!Mathf.Approximately(text.alpha, target))
        {
            float a = Mathf.MoveTowards(text.alpha, target, Time.unscaledDeltaTime / time);
            text.alpha = a;
            yield return null;
        }
    }
}
