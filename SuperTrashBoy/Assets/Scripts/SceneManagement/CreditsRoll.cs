using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{

    [SerializeField] ScrollRect scrollerView;
    [SerializeField] float scrollFactor = 30f;

    // Auto hides on start
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Begin(Fader f)
    {
        scrollerView.verticalScrollbar.SetValueWithoutNotify(1);
        scrollerView.verticalScrollbar.value = 1;
        StartCoroutine(RollCredits());
    }

    private IEnumerator RollCredits()
    {
        WaitForEndOfFrame wf = new WaitForEndOfFrame();
        WaitForSeconds ws = new WaitForSeconds(1);
        Scrollbar vs = scrollerView.verticalScrollbar;

        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(2); // Wait a bit for the music to start
        while (vs.value >= 0)
        {
            vs.value -= Time.deltaTime / scrollFactor;
            //vs.SetValueWithoutNotify(vs.value - Time.deltaTime / scrollFactor);
            //Debug.Log(vs.value);
            yield return wf;
        }
        Invoke(nameof(Start), 2f);
        Cursor.lockState = CursorLockMode.Confined;

        yield return null;
    }
}
