using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outro : MonoBehaviour
{

    [SerializeField] ScrollRect scrollerView;
    [SerializeField] float scrollFactor = 20f;

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

        while (vs.value >= 0)
        {
            vs.value -= Time.deltaTime / scrollFactor;
            //vs.SetValueWithoutNotify(vs.value - Time.deltaTime / scrollFactor);
            Debug.Log(vs.value);
            yield return wf;
        }
        Invoke(nameof(Start), 2f);
        yield return null;
    }
}
