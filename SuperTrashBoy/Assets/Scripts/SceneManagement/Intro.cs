using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;

    [SerializeField] private float panel1EnterTime = 1f;
    [SerializeField] private float panel1FadeInTime = 0.5f;
    [SerializeField] private float panel1PersistanceTime = 1f;
    [SerializeField] private float panel1FadeOutTime = 0.5f;

    [SerializeField] private float panel2EnterTime = 1f;
    [SerializeField] private float panel2FadeInTime = 0.5f;
    [SerializeField] private float panel2PersistanceTime = 1f;
    [SerializeField] private float panel2FadeOutTime = 0.5f;
    [SerializeField] private float mainMenuTime = 20f;
    [SerializeField] private float mainMenuFadeInTime = 0.3f;

    private TextSizeIncreaser panel1text;
    private TextSizeIncreaser panel2text;

    private void Start()
    {
        //panel1.SetActive(false);
        //panel2.SetActive(false);

        panel1text = panel1.GetComponent<TextSizeIncreaser>();
        panel2text = panel2.GetComponent<TextSizeIncreaser>();

        // Disable all children of the Intro object
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void Begin(Fader fader)
    {
        //StartCoroutine(IntroSequence(fader));
        StartCoroutine(DynamicIntroSequence(fader));
    }

    private IEnumerator DynamicIntroSequence(Fader fader)
    {
        fader.FadeOutImmediate();

        foreach (Transform t in transform)
        {
            GameObject thePanel = t.gameObject;
            NarrationPanel theNarration = t.GetComponent<NarrationPanel>();
            if (null == theNarration)
            {
                continue;
            }
            else
            {
                thePanel.SetActive(true);
                TextSizeIncreaser thePanelText = thePanel.GetComponent<TextSizeIncreaser>();
                thePanelText.BeginIncrease();
                yield return new WaitForSeconds(theNarration.enterTime);
                yield return fader.FadeIn(theNarration.fadeInTime);
                yield return new WaitForSeconds(theNarration.persistanceTime);
                yield return fader.FadeOut(theNarration.fadeOutTime);
                thePanelText.ResetState();
                thePanel.SetActive(false);
            }
        }

        yield return new WaitForSeconds(mainMenuTime);
        yield return fader.FadeIn(mainMenuFadeInTime);
    }

    private IEnumerator IntroSequence(Fader fader)
    {
        fader.FadeOutImmediate();
        yield return new WaitForSeconds(panel1EnterTime);
        panel1.SetActive(true);
        panel1text.BeginIncrease();
        yield return fader.FadeIn(panel1FadeInTime);
        yield return new WaitForSeconds(panel1PersistanceTime);
        yield return fader.FadeOut(panel1FadeOutTime);
        panel1text.ResetState();
        panel1.SetActive(false);
        panel2.SetActive(true);
        panel2text.BeginIncrease();
        yield return new WaitForSeconds(panel2EnterTime);
        yield return fader.FadeIn(panel2FadeInTime);
        yield return new WaitForSeconds(panel2PersistanceTime);
        yield return fader.FadeOut(panel2FadeOutTime);
        panel2text.ResetState();
        panel2.SetActive(false);
        yield return new WaitForSeconds(mainMenuTime);
        yield return fader.FadeIn(mainMenuFadeInTime);
    }
}
