using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] float updateVelocity = 2f;

    public Vector3 offset = Vector3.zero;
    public Health enemyHealth;
    public bool followsObject = true;
    public bool hasSetPos = false;
    public Vector3 screenPos = Vector3.zero;

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
        if (followsObject)
        {
            //Make sure object is on screen, and if so show the healthbar on the object
            if (enemyHealth.GetComponentInChildren<Renderer>() != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    DontShow();
                    return;
                }
                Vector3 viewing = Camera.main.WorldToViewportPoint(enemyHealth.gameObject.transform.position);
                bool showingObject = false;
                if (viewing.x > 0 && viewing.x < 1 && viewing.y > 0 && viewing.y < 1 && viewing.z > Camera.main.nearClipPlane)
                {
                    showingObject = true;
                }
                Physics.Raycast(enemyHealth.gameObject.transform.position, player.transform.position - enemyHealth.gameObject.transform.position, out RaycastHit hit);
                if (hit.collider == null)
                {
                    DontShow();
                }
                else if (hit.collider.gameObject == player.GetComponent<Collider>().gameObject
                    && enemyHealth.GetComponentInChildren<Renderer>().isVisible
                    && showingObject == true)
                {
                    //Only show if object is in camera and player has LOS
                    transform.position = Camera.main.WorldToScreenPoint(enemyHealth.gameObject.transform.position + offset);
                }
                else
                {
                    DontShow();
                }
            }
            else
            {
                DontShow();
            }
        }
        else
        {
            if (!hasSetPos)
                transform.position = screenPos;
        }

    }

    void DontShow()
    {
        //Make sure this is not on screen
        transform.position = Camera.main.ViewportToScreenPoint(new Vector3(-100, -100));
    }
}
