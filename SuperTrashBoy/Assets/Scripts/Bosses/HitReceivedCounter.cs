using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitReceivedCounter : MonoBehaviour
{
    private int hitReceived = 0;
    public event Action onHit;
    public UnityEvent onHitEvent;

    public bool canOnlyBeHitByPlayer = false;

    public void Hit()
    {
        hitReceived ++;
        onHit?.Invoke();
        onHitEvent?.Invoke();
    }

    public void Hit(Transform damager)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (canOnlyBeHitByPlayer && damager.gameObject != player) return;
        else Hit();
    }

    public int GetHitReceivedNumber()
    {
        return hitReceived;
    }

    public void ResetHits()
    {
        hitReceived = 0;
    }
}
