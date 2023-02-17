using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuGemController : MonoBehaviour
{
    private Health health;
    private KaijuController controller;
    // Start is called before the first frame update

    void Start()
    {
        controller = FindObjectOfType<KaijuController>();
        health = GetComponent<Health>();
        health.onTakeDamage += HandleHit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleHit(float amount, Transform damager)
    {
        controller.HandleGemDamage(gameObject.transform.parent.gameObject);
    }
}
