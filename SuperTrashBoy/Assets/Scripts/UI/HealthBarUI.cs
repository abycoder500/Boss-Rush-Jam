using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] float updateVelocity = 2f;

    private Health playerHealth;

    private void Awake() 
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();    
    }

    private void Start() 
    {
        UpdateUI(true);    
    }

    private void OnEnable() 
    {
        playerHealth.onTakeDamage += UpdateUI;
        playerHealth.onHeal += UpdateUI;
    }

    private void OnDisable()
    {
        playerHealth.onTakeDamage -= UpdateUI;
        playerHealth.onHeal -= UpdateUI;
    }
    

    private void UpdateUI(float amount)
    {
        Debug.Log("Update UI");
        StartCoroutine(UpdateHealthBar());
    }

    private void UpdateUI(bool immediate)
    {
        if(immediate) healthBarImage.fillAmount = playerHealth.GetHealthFraction();
        else UpdateUI(1f);
    }

    private void UpdateUI(float amount, Transform damager)
    {
        UpdateUI(amount);
    }

    private IEnumerator UpdateHealthBar()
    {
        Debug.Log(healthBarImage.fillAmount <= playerHealth.GetHealthFraction());
        while(!Mathf.Approximately(healthBarImage.fillAmount, playerHealth.GetHealthFraction()))
        {
            healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, playerHealth.GetHealthFraction(), updateVelocity * Time.deltaTime);
            yield return null;
        }
        yield break;
    }

}
