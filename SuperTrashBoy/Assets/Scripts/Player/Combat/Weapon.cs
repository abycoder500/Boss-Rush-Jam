using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    [SerializeField] private GameObject weaponPrefab;

    public abstract void Attack(GameObject instigator);

    public GameObject GetWeaponPrefab()
    {
        return weaponPrefab;
    }
}
