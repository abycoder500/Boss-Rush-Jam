using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitReceivedCounter : MonoBehaviour
{
    private int hitReceived = 0;
    public event Action onHit;

    public void Hit()
    {
        hitReceived ++;
        onHit?.Invoke();
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
