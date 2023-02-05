using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    private Health health;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        health = GetComponent<Health>();
        health.OnDeath.AddListener(DestroyParent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
