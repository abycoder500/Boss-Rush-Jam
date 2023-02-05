using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] float updateVelocity = 2f;

    public Health enemyHealth;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI(true);
    }

    private void OnEnable()
    {
        enemyHealth.onTakeDamage += UpdateUI;
        enemyHealth.onHeal += UpdateUI;
    }

    private void OnDisable()
    {
        enemyHealth.onTakeDamage -= UpdateUI;
        enemyHealth.onHeal -= UpdateUI;
    }

    private void UpdateUI(float amount)
    {
        Debug.Log("Update UI");
        StartCoroutine(UpdateHealthBar());
    }

    private void UpdateUI(bool immediate)
    {
        if (immediate) healthBarImage.fillAmount = enemyHealth.GetHealthFraction();
        else UpdateUI(1f);
    }

    private void UpdateUI(float amount, Transform damager)
    {
        UpdateUI(amount);
    }

    private IEnumerator UpdateHealthBar()
    {
        Debug.Log(healthBarImage.fillAmount <= enemyHealth.GetHealthFraction());
        while (healthBarImage.fillAmount > enemyHealth.GetHealthFraction())
        {
            healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, enemyHealth.GetHealthFraction(), updateVelocity * Time.deltaTime);
            yield return null;
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
