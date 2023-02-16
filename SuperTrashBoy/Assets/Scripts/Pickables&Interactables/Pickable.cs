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

    protected virtual void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player.name);
        if (lifeTime > 0)
        {
            Destroy(this.gameObject, lifeTime);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("trigger");
        if(interactWithPlayerOnly)
        {
            Debug.Log(other.name);
            Debug.Log(player.name);
            if (other.gameObject == player)
            {
                Debug.Log("player");
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
