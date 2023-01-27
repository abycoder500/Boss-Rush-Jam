using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon: MonoBehaviour
{
    [SerializeField] protected AnimationClip attackAnimation;
    [SerializeField] protected float damage;

    public abstract void Attack(GameObject instigator, Action AttackFinished);
}
