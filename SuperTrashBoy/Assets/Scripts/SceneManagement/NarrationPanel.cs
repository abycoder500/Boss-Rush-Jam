using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationPanel : MonoBehaviour
{
    // This should really be done as a Scriptable Object...
    [SerializeField] private float enterTime;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float persistanceTime;
    [SerializeField] private float fadeOutTime;

    public float EnterTime { get => enterTime; }
    public float FadeInTime { get => fadeInTime; }
    public float PersistanceTime { get => persistanceTime; }
    public float FadeOutTime { get => fadeOutTime; }
}
