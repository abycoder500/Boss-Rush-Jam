using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossNameUI : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] TextMeshProUGUI bossNameText;
    [SerializeField] float timeToShowPanel = 1f;
    [SerializeField] float delayPanelBoss = 0.3f;
    [SerializeField] float persitanceTimeOnScreen = 3f;
    [SerializeField] float timeToShowBossName = 2f;
    [SerializeField] float disappearanceTime = 0.5f;
    [SerializeField] float targetPanelAlpha = 0.9f;

    private DummyBT dummyBT;
    private FightManager jackFightManager;

    private void Awake() 
    {
        dummyBT = FindObjectOfType<DummyBT>();
        jackFightManager = FindObjectOfType<FightManager>();
    }

    private void OnEnable() 
    {
        if (dummyBT != null) dummyBT.onActivate += ShowBossName;
        if (jackFightManager != null) jackFightManager.onActivate += ShowBossName;
    }

    private void OnDisable()
    {
        if (dummyBT != null) dummyBT.onActivate -= ShowBossName;
        if (jackFightManager != null) jackFightManager.onActivate -= ShowBossName;
    }

    private void Start() 
    {
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        bossNameText.alpha = 0;    
    }

    public void ShowBossName(string bossName)
    {
        StartCoroutine(ShowText(persitanceTimeOnScreen, bossName));
    }

    private IEnumerator ShowText(float timeOnScreen, string bossName)
    {
        bossNameText.text = bossName;
        StartCoroutine(ChangePanelAlpha(targetPanelAlpha, timeToShowPanel));
        yield return new WaitForSeconds(delayPanelBoss);
        StartCoroutine(ChangeTextAlpha(1f, timeToShowBossName));
        yield return new WaitForSeconds(timeOnScreen);
        StartCoroutine(ChangePanelAlpha(0f, disappearanceTime));
        StartCoroutine(ChangeTextAlpha(0f, disappearanceTime));
    }

    private IEnumerator ChangePanelAlpha(float target, float time)
    {
        while(!Mathf.Approximately(panel.color.a, target))
        {
            float a = Mathf.MoveTowards(panel.color.a, target, Time.unscaledDeltaTime/time);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, a);
            yield return null;
        }
    }

    private IEnumerator ChangeTextAlpha(float target, float time)
    {
        while (!Mathf.Approximately(bossNameText.alpha, target))
        {
            float a = Mathf.MoveTowards(bossNameText.alpha, target, Time.unscaledDeltaTime / time);
            bossNameText.alpha = a;
            yield return null;
        }
    }
}
