using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    [SerializeField] private bool interactWithPlayerOnly = true;

    protected GameObject player;

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Triggered");
        if(interactWithPlayerOnly)
        {
            if (other.gameObject == player)
            {
                Collect();
            }
        }
        else
        {
            Collect();
        }    
    }

    protected abstract void Collect();
}
