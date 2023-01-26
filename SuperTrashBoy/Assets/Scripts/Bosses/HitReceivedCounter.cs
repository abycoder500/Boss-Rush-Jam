using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitReceivedCounter : MonoBehaviour
{
    private int hitReceived = 0;

    public void Hit()
    {
        hitReceived ++;
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
