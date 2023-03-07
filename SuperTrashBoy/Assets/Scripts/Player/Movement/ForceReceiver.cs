using System.Collections.Generic;
using UnityEngine;

public class ForceReceiver : MonoBehaviour 
{
    private Vector3 finalforce = Vector3.zero;

    public void AddForce(Vector3 forceToAdd)
    {
        finalforce += forceToAdd;
    }

    public void RemoveForce(Vector3 forceToRemove)
    {
        finalforce -= forceToRemove;
    }

    public Vector3 GetForce()
    {
        return finalforce;
    }

}