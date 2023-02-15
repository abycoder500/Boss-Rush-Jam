using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outro : MonoBehaviour
{
    [SerializeField] private float mainMenuTime = 20f;
    [SerializeField] private float mainMenuFadeInTime = 0.3f;

    [Space(1)]
    [SerializeField] CreditsRoll creditsRoll;

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
        StartCoroutine(DynamicOutroSequence(fader));
    }

    protected IEnumerator DynamicOutroSequence(Fader fader)
    {
        fader.FadeOutImmediate();
        Cursor.lockState = CursorLockMode.Locked;

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
        if (null != creditsRoll)
        {
            creditsRoll.gameObject.SetActive(true);
            creditsRoll.Begin(fader);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Invoke(nameof(Start), 1f);
        }
    }


}
