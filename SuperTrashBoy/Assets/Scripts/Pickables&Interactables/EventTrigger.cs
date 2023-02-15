using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] GameObject visuals;
    [SerializeField] bool deactivateOnTrigger = true;

    public UnityEvent OnTriggerEvent;
    private GameObject player;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");    
        visuals.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {   
        if(other.gameObject != player) return;
 
        OnTriggerEvent?.Invoke();
        if(deactivateOnTrigger) this.gameObject.SetActive(false);
    }
}
