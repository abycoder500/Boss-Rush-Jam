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

    public void Hit()
    {
        hitReceived ++;
        onHit?.Invoke();
        onHitEvent?.Invoke();
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
