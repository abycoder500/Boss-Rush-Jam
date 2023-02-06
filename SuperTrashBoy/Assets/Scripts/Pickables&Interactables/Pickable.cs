using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Pickable : MonoBehaviour
{
    [SerializeField] private bool interactWithPlayerOnly = true;
    [SerializeField] private float lifeTime = -1;

    
    public UnityEvent OnPicked;

    protected GameObject player;

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (lifeTime > 0)
        {
            Destroy(this.gameObject, lifeTime);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
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

    protected virtual void Collect()
    {
        OnPicked?.Invoke();
        Destroy(this.gameObject);
    }


}
