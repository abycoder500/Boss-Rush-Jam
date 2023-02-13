using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyRageUI : MonoBehaviour
{
    [SerializeField] private GameObject rageBar;
    [SerializeField] private Image rageFillBar;
    [SerializeField] private DummyBT dummy;
    [SerializeField] private float updateVelocity = 20f;

    private void Start() 
    {
        rageBar.SetActive(false);
        rageFillBar.fillAmount = 0f;    
    }

    private void OnEnable() 
    {
        dummy.onRageChange += UpdateUI;    
        dummy.onAlive += () => rageBar.SetActive(true);
        dummy.onDie += () => rageBar.SetActive(false);
    }

    private void OnDisable()
    {
        dummy.onRageChange -= UpdateUI;
        dummy.onAlive -= () => rageBar.SetActive(true);
        dummy.onDie -= () => rageBar.SetActive(false);
    }

    private void UpdateUI(float amount)
    {
        Debug.Log("updating to " + amount);
        StartCoroutine(UpdateBar(amount));
    }

    private IEnumerator UpdateBar(float amount)
    {
        Debug.Log(rageFillBar.fillAmount <= amount);
        while (!Mathf.Approximately(rageFillBar.fillAmount, amount))
        {
            rageFillBar.fillAmount = Mathf.MoveTowards(rageFillBar.fillAmount, amount, updateVelocity * Time.unscaledDeltaTime);
            yield return null;
        }
        yield break;
    }
}
