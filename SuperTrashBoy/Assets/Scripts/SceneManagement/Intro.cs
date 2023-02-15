using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] private float mainMenuTime = 20f;
    [SerializeField] private float mainMenuFadeInTime = 0.3f;


    private void Start()
    {
        // Disable all children of the Intro object
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void Begin(Fader fader)
    {
        StartCoroutine(DynamicIntroSequence(fader));
    }

    protected IEnumerator DynamicIntroSequence(Fader fader)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
                yield return new WaitForSeconds(theNarration.EnterTime);
                yield return fader.FadeIn(theNarration.FadeInTime);
                yield return new WaitForSeconds(theNarration.PersistanceTime);
                yield return fader.FadeOut(theNarration.FadeOutTime);
                thePanelText.ResetState();
                thePanel.SetActive(false);
            }
        }

        yield return new WaitForSeconds(mainMenuTime);
        yield return fader.FadeIn(mainMenuFadeInTime);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Invoke(nameof(Start), 1f);
    }

}
